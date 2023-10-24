namespace Domain.Models;

public class Director
{
    public Director()
    {
        Movies = new List<Movie>();
    }

    public Director(Guid id, string name)
    {
        Id = id;
        Name = name;
    }


    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Movie> Movies { get; set; } = null!;
}