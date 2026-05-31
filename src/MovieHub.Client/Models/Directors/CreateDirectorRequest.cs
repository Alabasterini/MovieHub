namespace MovieHub.Client.Models.Directors;

public class CreateDirectorRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Nationality { get; set; }
}
