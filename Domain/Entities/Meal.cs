using LiteDB;

namespace Edamam.Domain.Entities;

/// <summary>
/// Represents a meal (can contain one or multiple recipes).
/// Encapsulates meal details and polymorphic processing.
/// </summary>
public class Meal
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<Recipe> Recipes { get; set; } = new();
    public NutritionalMetric? Nutritionals { get; set; }
    public DateTime MealDate { get; set; } = DateTime.UtcNow;
    public MealType Type { get; set; } = MealType.Lunch;

    public override string ToString() => $"Meal: {Name} ({Recipes.Count} recipes) - {MealDate:g}";
}

/// <summary>
/// Enum for meal classification (Polymorphism support).
/// </summary>
public enum MealType
{
    Breakfast,
    Brunch,
    Lunch,
    Snack,
    Dinner,
    Supper
}
