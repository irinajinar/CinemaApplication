using Domain.Models;

namespace ClassLibrary1.RepositoryInterfaces;

public interface IActorRepository
{
    Task AddActorAsync(Actor? actor);
    Task<Actor?> GetByIdAsync(Guid actorId);
    Task<List<Actor?>> GetAllActorsAsync();
    Task<bool> DeleteActorAsync(Guid actorId);
    Task<Actor?> UpdateActorAsync(Guid actorId, Actor updatedActor);
    Task<Actor?> GetActorWithMoviesAsync(Guid actorId);
}