using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Movie
{
    public Movie()
    {
        
    }
    public Movie(Guid id, string name, string description, int year)
    {
        Id = id;
        Name = name;
        Description = description;
        Year = year;
    }
    
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
    public Guid DirectorId { get; set; }
    
    public Director Director { get; set; }
}