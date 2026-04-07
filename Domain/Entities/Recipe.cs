using LiteDB;

namespace TEST.Domain.Entities;

/// <summary>
/// Represents a recipe (a collection of ingredients).
/// Encapsulates recipe details and ingredients.
/// </summary>
public class Recipe
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Ingredient> Ingredients { get; set; } = new();
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public override string ToString() => $"Recipe: {Name} ({Ingredients.Count} ingredients)";
}
