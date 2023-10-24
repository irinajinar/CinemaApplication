using ClassLibrary1.Dtos;

namespace ClassLibrary1.BodyRequest;

public class MovieWithActorsRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
    public Guid DirectorId { get; set; }
    public List<ActorBodyRequest> Actors { get; set; }
}