namespace ClassLibrary1.Responses;

public class DeleteManyResponse
{
    public string Message { get; set; }

    public DeleteManyResponse(string message)
    {
        Message = message;
    }
}