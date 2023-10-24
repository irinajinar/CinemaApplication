namespace ClassLibrary1.Dtos;

public class ActorDto
{
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public string Country { get; set; } = null!;
    public string Biography { get; set; } = null!;
    public Guid MovieId { get; set; }
}