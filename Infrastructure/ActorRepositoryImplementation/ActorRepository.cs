using ClassLibrary1.RepositoryInterfaces;
using Domain.Models;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ActorRepositoryImplementation;

public class ActorRepository : IActorRepository
{
    private readonly DataAppContext _dataAppContext;

    public ActorRepository(DataAppContext dataAppContext)
    {
        _dataAppContext = dataAppContext;
    }

    public async Task AddActorAsync(Actor? actor)
    {
        await _dataAppContext.Actors.AddAsync(actor!);
        await _dataAppContext.SaveChangesAsync();
    }

    public async Task<Actor?> GetByIdAsync(Guid actorId)
    {
        return await _dataAppContext.Actors.FindAsync(actorId);
    }

    public async Task<List<Actor?>> GetAllActorsAsync()
    {
        var actors = await _dataAppContext.Actors
            .Include(actor => actor.Movies)
            .ToListAsync();

        return actors!;
    }

    public async Task<bool> DeleteActorAsync(Guid actorId)
    {
        var actor = await _dataAppContext.Actors.FindAsync(actorId);
        if (actor == null)
        {
            return false;
        }

        _dataAppContext.Actors.Remove(actor);
        await _dataAppContext.SaveChangesAsync();
        return true;
    }

    public async Task<Actor?> UpdateActorAsync(Guid actorId, Actor updatedActor)
    {
        var actor = await _dataAppContext.Actors.FindAsync(actorId);
        if (actor == null)
        {
            return null;
        }

        actor.Name = updatedActor.Name;
        actor.Age = updatedActor.Age;
        actor.Country = updatedActor.Country;
        actor.Biography = updatedActor.Biography;
        actor.Movies = updatedActor.Movies;
        await _dataAppContext.SaveChangesAsync();
        return actor;
    }

    public async Task<Actor?> GetActorWithMoviesAsync(Guid actorId)
    {
        return await _dataAppContext.Actors
            .Include(a => a.Movies)
            .FirstOrDefaultAsync(a => a.Id == actorId);
    }
}