using ClassLibrary1.BodyRequest;
using ClassLibrary1.Dtos;
using ClassLibrary1.RepositoryInterfaces;
using ClassLibrary1.Responses;
using ClassLibrary1.ServiceInterface;
using Domain.Exceptions;
using Domain.Models;

namespace ClassLibrary1.ServiceImplementation;

public class MovieService: IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }
    public async Task<MovieResponse> CreateMovieAsync(MovieDto movieDto)
    {
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(movieDto.Name))
        {
            validationErrors.Add("Name is required.");
        }
        if (string.IsNullOrWhiteSpace(movieDto.Description))
        {
            validationErrors.Add("Description is required.");
        }
        if (validationErrors.Count > 0)
        {
            throw new MultiValidationException(validationErrors);
        }
        var newMovie = new Movie(Guid.NewGuid(), movieDto.Name, movieDto.Description);
        await _movieRepository.AddMovieAsync(newMovie);
        return new MovieResponse(newMovie.Id, newMovie.Name, newMovie.Description);
    }

    public async Task<MovieResponse> GetByIdAsync(Guid movieId)
    {
      var movie = await _movieRepository.GetByIdAsync(movieId);
        if (movie == null)
        {
            throw new MultiValidationException($"The movie with the {movieId} not found");
        }
        return new MovieResponse(
            movie.Id,
            movie.Name,
            movie.Description
        );
    }
 
    public async Task<List<MovieResponse>> GetAllMoviesAsync(string? filterByName = null)
    {
        var movies = await _movieRepository.GetAllMoviesAsync();
        if (!string.IsNullOrEmpty(filterByName))
        {
            movies = movies.Where(movies => movies.Name.Contains(filterByName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var movieResponses = movies.Select(movie => new MovieResponse(
            movie.Id,
            movie.Name,
            movie.Description)).ToList();
        return movieResponses;
    }

    public async Task<MovieResponse> UpdateMovieAsync(Guid movieId, MovieBodyRequest movieBodyRequest)
    {
        var existingMovie = await _movieRepository.GetByIdAsync(movieId);
        if (existingMovie == null)
        {
            throw new MultiValidationException("Movie not found");
        }

        existingMovie.Name = movieBodyRequest.Name;
        existingMovie.Description = movieBodyRequest.Description;
        var updatedMovie = await _movieRepository.UpdateMovieAsync(movieId, existingMovie);
        if (updatedMovie == null)
        {
            throw new MultiValidationException("Failed to update the movie.");
        }

        return new MovieResponse(updatedMovie.Id, updatedMovie.Name, updatedMovie.Description);
    }
    public async Task DeleteAsync(Guid movieId)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId);

        if (movie == null)
        {
            return;
        }

        var isDeleted = await _movieRepository.DeleteMovieAsync(movieId);

        if (!isDeleted)
        {
            throw new MultiValidationException("Failed to delete the movie.");
        }
    }
    public async Task<DeleteManyResponse> DeleteMoviesAsync(List<string> movieIds)
    {
        var isDeleted = await _movieRepository.DeleteMoviesAsync(movieIds);

        string message = isDeleted ? "Deleted all movies." : "No movies were deleted.";

        return new DeleteManyResponse(message);
    }
    
    
}