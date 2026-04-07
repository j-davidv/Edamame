using TEST.Domain.Entities;

namespace TEST.Domain.Interfaces;

/// <summary>
/// Abstraction for nutrition analysis service (Dependency Inversion).
/// Defines contract for AI-based nutritional analysis.
/// </summary>
public interface INutritionAnalysisService
{
    /// <summary>
    /// Analyzes a meal and returns its nutritional composition.
    /// </summary>
    Task<NutritionalMetric> AnalyzeMealAsync(Meal meal);

    /// <summary>
    /// Analyzes a recipe and returns its nutritional composition.
    /// </summary>
    Task<NutritionalMetric> AnalyzeRecipeAsync(Recipe recipe);

    /// <summary>
    /// Gets dietary advice for a meal.
    /// </summary>
    Task<string> GetDietaryAdviceAsync(Meal meal);
}
