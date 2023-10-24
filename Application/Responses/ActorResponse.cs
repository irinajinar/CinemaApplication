using Domain.Models;

namespace ClassLibrary1.Responses;

public class ActorResponse
{
    public ActorResponse(Actor? actor, bool includeMovies = false)
    {
        if (actor == null) return;
        Id = actor.Id;
        Name = actor.Name;
        Age = actor.Age;
        Country = actor.Country!;
        Biography = actor.Biography!;
        if (includeMovies)
        {
            MovieIds = actor.Movies.Select(m => m.Id).ToList();
        }
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public string Country { get; set; } = null!;
    public string Biography { get; set; } = null!;
    public List<Guid> MovieIds { get; set; } = new List<Guid>();
}