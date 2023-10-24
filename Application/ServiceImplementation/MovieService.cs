using ClassLibrary1.BodyRequest;
using ClassLibrary1.RepositoryInterfaces;
using ClassLibrary1.Responses;
using ClassLibrary1.ServiceInterface;
using Domain.Exceptions;
using Domain.Models;

namespace ClassLibrary1.ServiceImplementation;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IDirectorRepository _directorRepository;
    private readonly IActorRepository _actorRepository;

    public MovieService(IMovieRepository movieRepository, IDirectorRepository directorRepository,
        IActorRepository actorRepository)
    {
        _movieRepository = movieRepository;
        _directorRepository = directorRepository;
        _actorRepository = actorRepository;
    }

    public async Task<MovieResponse> CreateMovieAsync(MovieWithActorsRequest movieRequest)
    {
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(movieRequest.Name))
        {
            validationErrors.Add("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(movieRequest.Description))
        {
            validationErrors.Add("Description is required.");
        }

        if (movieRequest.Year <= 0 || movieRequest.Year > DateTime.Now.Year)
        {
            validationErrors.Add("Year must be a valid positive year up to the current year.");
        }

        if (movieRequest.DirectorId == Guid.Empty)
        {
            validationErrors.Add("Director Id is required.");
        }

        if (validationErrors.Count > 0)
        {
            throw new MultiValidationException(validationErrors);
        }

        var newMovie = new Movie(Guid.NewGuid(), movieRequest.Name, movieRequest.Description, movieRequest.Year);
        Director director = await _directorRepository.GetByIdAsync(movieRequest.DirectorId);
        if (director == null)
        {
            throw new MultiValidationException("Director not found. You have to add the Director first!");
        }

        newMovie.DirectorId = director.Id;

        var actors = new List<Actor>();
        foreach (var actorRequest in movieRequest.Actors)
        {
            var existingActor = await _actorRepository.GetByIdAsync(actorRequest.Id);

            if (existingActor != null)
            {
                actors.Add(existingActor);
            }
            else
            {
                var actor = new Actor(Guid.NewGuid(), actorRequest.Name, actorRequest.Age, actorRequest.Country!,
                    actorRequest.Biography!);
                actors.Add(actor);
            }
        }

        newMovie.Actors = actors!;
        await _movieRepository.AddMovieAsync(newMovie);

        var movieResponse = new MovieResponse(newMovie, newMovie.DirectorId, includeActors: true);
        return movieResponse;
    }

    public async Task<MovieResponse> GetByIdAsync(Guid movieId)
    {
        var movie = await _movieRepository.GetMovieWithActorsAsync(movieId);
        if (movie == null)
        {
            throw new MultiValidationException($"The movie with the {movieId} not found");
        }

        var movieResponse = new MovieResponse(movie, movie.DirectorId, includeActors: true);

        return movieResponse;
    }

    public async Task<List<MovieResponse>> GetAllMoviesAsync(string? filterByName = null)
    {
        var movies = await _movieRepository.GetAllMoviesAsync();

        if (!string.IsNullOrEmpty(filterByName))
        {
            movies = movies.Where(movie => movie!.Name.Contains(filterByName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var movieResponses = movies.Select(movie =>
        {
            var movieResponse = new MovieResponse(movie, movie!.DirectorId, includeActors: true);


            return movieResponse;
        }).ToList();

        return movieResponses;
    }

    public async Task<MovieResponse> UpdateMovieAsync(Guid movieId, MovieBodyRequest movieBodyRequest)
    {
        var existingMovie = await _movieRepository.GetMovieWithActorsAsync(movieId);
        if (existingMovie == null)
        {
            throw new MultiValidationException("Movie not found");
        }

        existingMovie.Name = movieBodyRequest.Name;
        existingMovie.Description = movieBodyRequest.Description;
        existingMovie.Year = movieBodyRequest.Year;
      //  existingMovie.DirectorId = movieBodyRequest.DirectorId;

        foreach (var actorIdToRemove in movieBodyRequest.ActorsToRemove)
        {
            var actorToRemove = existingMovie.Actors.FirstOrDefault(a => a!.Id == actorIdToRemove);
            if (actorToRemove != null)
            {
                existingMovie.Actors.Remove(actorToRemove);
            }
            else
            {
                throw new MultiValidationException(
                    $"The actor with ID {actorIdToRemove} was not found in the movie's actor list.");
            }
        }

        foreach (var actorDto in movieBodyRequest.ActorsToAdd)
        {
            if (!string.IsNullOrWhiteSpace(actorDto.Name))
            {
                var existingActor = existingMovie.Actors.FirstOrDefault(a => a!.Id == actorDto.Id);

                if (existingActor == null)
                {
                    // The actor is not already in the collection, so add it.
                    existingActor = await _actorRepository.GetByIdAsync(actorDto.Id);

                    if (existingActor != null)
                    {
                        existingMovie.Actors.Add(existingActor);
                    }else
                    {
                        // If the actor doesn't exist, create a new one and add it.
                        var newActor = new Actor(Guid.NewGuid(), actorDto.Name, actorDto.Age, actorDto.Country, actorDto.Biography);
                        existingMovie.Actors.Add(newActor);
                    }
                }
            }
        }

        await _movieRepository.UpdateMovieAsync(movieId, existingMovie);

        var updatedMovieResponse = new MovieResponse(existingMovie, existingMovie.DirectorId, includeActors: true);

        return updatedMovieResponse;
    }


    public async Task DeleteAsync(Guid movieId)
    {
        await _movieRepository.GetByIdAsync(movieId);

        await _movieRepository.DeleteMovieAsync(movieId);
    }

    public async Task<DeleteManyResponse> DeleteMoviesAsync(List<string> movieIds)
    {
        var isDeleted = await _movieRepository.DeleteMoviesAsync(movieIds);

        var message = isDeleted ? "Deleted all movies." : "No movies were deleted.";

        return new DeleteManyResponse(message);
    }
}