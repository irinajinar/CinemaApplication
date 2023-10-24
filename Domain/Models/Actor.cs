namespace Domain.Models;

public class Actor
{
    public Actor()
    {
    }

    public Actor(Guid id, string name, int age, string country, string biography)
    {
        Id = id;
        Name = name;
        Age = age;
        Country = country;
        Biography = biography;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public string? Country { get; set; }
    public string? Biography { get; set; }
    
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}