using LiteDB;

namespace Edamam.Domain.Entities;

/// encapsulattons of meal details and polymorphic processing.

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

/// enums for polymorphication support
public enum MealType
{
    Breakfast,
    Brunch,
    Lunch,
    Snack,
    Dinner,
    Supper
}
