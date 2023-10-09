using Domain.Models;
namespace ClassLibrary1.Dtos;

public class MovieDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
    public Guid DirectorId { get; set; }
}