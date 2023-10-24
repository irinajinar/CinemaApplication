using Domain.Models;

namespace ClassLibrary1.Responses;

public class MovieResponse
{
    public MovieResponse(Movie? movie,
        Guid directorId, bool includeActors)
    {
        if (movie == null) return;
        MovieId = movie.Id;
        MovieName = movie.Name;
        MovieDescription = movie.Description;
        YearOfProduction = movie.Year;
        DirectorId = directorId;
        if (includeActors)
        {
            ActorIds = movie.Actors.Select(a => a!.Id).ToList();
        }
    }

    public Guid MovieId { get; private set; }
    public string MovieName { get; private set; } = null!;
    public string MovieDescription { get; private set; } = null!;
    public int YearOfProduction { get; set; }
    public Guid DirectorId { get; set; }
    public List<Guid> ActorIds { get; set; } = new List<Guid>();
}