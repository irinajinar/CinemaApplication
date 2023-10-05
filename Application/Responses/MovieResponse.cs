namespace ClassLibrary1.Responses;

public class MovieResponse
{
    
    public MovieResponse(Guid newMovieId, string newMovieName, string newMovieDescription)
    {
        MovieId = newMovieId;
        MovieName = newMovieName;
        MovieDescription = newMovieDescription;
    }

    public Guid MovieId { get; private set; }
    public string MovieName { get; private set; } = null!;
    public string MovieDescription { get; private set; } = null!;
    
}