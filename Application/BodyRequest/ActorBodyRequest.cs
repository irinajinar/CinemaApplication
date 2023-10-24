namespace ClassLibrary1.BodyRequest;

public class ActorBodyRequest
{
    public ActorBodyRequest(string name, int age, string country, string biography, Guid id)
    {
        Name = name;
        Age = age;
        Country = country;
        Biography = biography;
        Id = id;
    }

    public string Name { get; set; }
    public int Age { get; set; }
    public string? Country { get; set; }
    public string? Biography { get; set; }
    public Guid Id { get; set; }
}