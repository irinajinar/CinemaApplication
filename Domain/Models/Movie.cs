using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Movie
{
    public Movie()
    {
        
    }
    public Movie(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
    
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}