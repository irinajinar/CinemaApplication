namespace ClassLibrary1.Responses;

public class MovieResponse
{
    public MovieResponse(Guid newMovieId, string newMovieName, string newMovieDescription, int newYearOfProduction,
        Guid directorId)
    {
        MovieId = newMovieId;
        MovieName = newMovieName;
        MovieDescription = newMovieDescription;
        YearOfProduction = newYearOfProduction;
        DirectorId = directorId;
    }

    public Guid MovieId { get; private set; }
    public string MovieName { get; private set; } = null!;
    public string MovieDescription { get; private set; } = null!;
    public int YearOfProduction { get; set; }
    public Guid DirectorId { get; set; }
}