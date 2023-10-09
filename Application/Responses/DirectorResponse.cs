namespace ClassLibrary1.Responses;

public class DirectorResponse
{
    public Guid DirectorId { get; set; }
    public string DirectorName { get; set; }

    public DirectorResponse(Guid id, string name)
    {
        DirectorId = id;
        DirectorName = name;
    }
}