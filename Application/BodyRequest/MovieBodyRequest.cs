namespace ClassLibrary1.BodyRequest;

public class MovieBodyRequest
{
    public MovieBodyRequest(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public MovieBodyRequest()
    {
        
    }

    public string Name { get; set; }
    public string Description { get; set; }
}