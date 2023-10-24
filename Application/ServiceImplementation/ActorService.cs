using ClassLibrary1.BodyRequest;
using ClassLibrary1.Dtos;
using ClassLibrary1.RepositoryInterfaces;
using ClassLibrary1.Responses;
using ClassLibrary1.ServiceInterface;
using Domain.Exceptions;
using Domain.Models;


namespace ClassLibrary1.ServiceImplementation;

public class ActorService : IActorService
{
    private readonly IActorRepository _actorRepository;
    private readonly IMovieRepository _movieRepository;

    public ActorService(IActorRepository actorRepository, IMovieRepository movieRepository)
    {
        _actorRepository = actorRepository;
        _movieRepository = movieRepository;
    }

    public async Task<ActorResponse> AddActorAsync(ActorDto actorDto)
    {
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(actorDto.Name))
        {
            validationErrors.Add("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(actorDto.Country))
        {
            validationErrors.Add("Country is required.");
        }

        if (string.IsNullOrWhiteSpace(actorDto.Biography))
        {
            validationErrors.Add("Biography is required.");
        }

        if (actorDto.Age < 0 || actorDto.Age > DateTime.Now.Year)
        {
            validationErrors.Add("Age must be a valid positive value");
        }

        if (actorDto.MovieId == Guid.Empty)
        {
            validationErrors.Add("Movie Id is required");
        }

        if (validationErrors.Count > 0)
        {
            throw new MultiValidationException(validationErrors);
        }

        var movie = await _movieRepository.GetByIdAsync(actorDto.MovieId);

        if (movie == null)
        {
            throw new MultiValidationException("Movie with the specified ID not found.");
        }

        var newActor = new Actor(Guid.NewGuid(), actorDto.Name, actorDto.Age, actorDto.Country, actorDto.Biography);
        movie.Actors.Add(newActor);

        await _actorRepository.AddActorAsync(newActor);

        var actorResponse = new ActorResponse(newActor, includeMovies: true);

        return actorResponse;
    }

    public async Task<List<ActorResponse>> GetAllActorsAsync(string? filterByName = null, Guid? filterByMovieId = null)
    {
        var actors = await _actorRepository.GetAllActorsAsync();

        if (!string.IsNullOrWhiteSpace(filterByName))
        {
            actors = actors.Where(a => a != null && a.Name.Contains(filterByName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (filterByMovieId.HasValue)
        {
            actors = actors.Where(a => a!.Movies.Any(m => m.Id == filterByMovieId.Value)).ToList();
        }

        var actorResponses = actors.Select(actor => new ActorResponse(actor, includeMovies: true)).ToList();
        return actorResponses;
    }

    public async Task<ActorResponse> GetByIdAsync(Guid actorId)
    {
        var actor = await _actorRepository.GetActorWithMoviesAsync(actorId);

        if (actor == null)
        {
            throw new MultiValidationException($"The actor with the ID {actorId} not found");
        }

        var actorResponse = new ActorResponse(actor, includeMovies: true);
        return actorResponse;
    }

    public async Task DeleteAsync(Guid actorId)
    {
        await _actorRepository.DeleteActorAsync(actorId);

        var isDeleted = await _actorRepository.DeleteActorAsync(actorId);
        if (!isDeleted)
        {
            throw new MultiValidationException("Failed to delete the actor.");
        }
    }

    public async Task<ActorResponse> UpdateActorAsync(Guid actorId, ActorBodyRequest actorBodyRequest)
    {
        var existingActor = await _actorRepository.GetByIdAsync(actorId);
        if (existingActor == null)
        {
            throw new MultiValidationException("Actor not found");
        }

        existingActor.Name = actorBodyRequest.Name;
        existingActor.Age = actorBodyRequest.Age;
        existingActor.Country = actorBodyRequest.Country;
        existingActor.Biography = actorBodyRequest.Biography;
        await _actorRepository.UpdateActorAsync(actorId, existingActor);
        return new ActorResponse(existingActor, includeMovies: true);
    }
}