namespace CookingApp.Data.Model;

public record Recipe
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required IEnumerable<string> Ingredients { get; set; }
    public required IEnumerable<string> Steps { get; set; }
}
