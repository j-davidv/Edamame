using LiteDB;

namespace TEST.Domain.Entities;

/// <summary>
/// Represents a single ingredient in a recipe.
/// Encapsulates ingredient details and quantity.
/// </summary>
public class Ingredient
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty; // e.g., "grams", "cups", "ml"
}
