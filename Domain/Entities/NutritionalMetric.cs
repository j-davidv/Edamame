namespace Edamam.Domain.Entities;

/// inherts from base
/// encapsulates calculated nutritional data
public class NutritionalMetric : NutritionalBase
{
    public string MealName { get; set; } = string.Empty;
    public DateTime AnalyzedDate { get; set; } = DateTime.UtcNow;
    public string? DietaryClassification { get; set; } // e.g., "Vegan", "Gluten-Free"
    public string? DietaryAdvice { get; set; }
    public int ServingSize { get; set; } = 1;

    public override string ToString()
    {
        return $"{MealName} - {Calories} kcal, Protein: {Protein}g, Carbs: {Carbohydrates}g, Fat: {Fat}g";
    }
}
