namespace CinemaApplication.Middleware;

public class ExceptionResponse
{
    public int StatusCode { get; set; }            
    public string? Message { get; set; }            
    public List<string> ValidationErrors { get; set; } = null!;
}