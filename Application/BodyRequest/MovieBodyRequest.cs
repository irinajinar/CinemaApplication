namespace ClassLibrary1.BodyRequest;

public class MovieBodyRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Year { get; set; }
    public Guid DirectorId { get; set; }
    public List<ActorBodyRequest> ActorsToAdd { get; set; } = new List<ActorBodyRequest>();
    public List<Guid> ActorsToRemove { get; set; } = new List<Guid>();
}