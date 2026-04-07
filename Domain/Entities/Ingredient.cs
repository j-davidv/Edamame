using LiteDB;

namespace Edamam.Domain.Entities;


/// encapsulation of ingredient details and quantity

public class Ingredient
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty; // e.g., "grams", "cups", "ml"
}
