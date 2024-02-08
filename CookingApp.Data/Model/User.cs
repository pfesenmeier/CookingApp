namespace CookingApp.Data.Model;

public record User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
}
