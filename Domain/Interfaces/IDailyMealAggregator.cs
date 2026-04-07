using Edamam.Domain.Entities;

namespace Edamam.Domain.Interfaces;

/// <summary>
/// Abstraction for daily meal aggregation logic.
/// </summary>
public interface IDailyMealAggregator
{
    /// <summary>
    /// Gets all meals for a specific date.
    /// </summary>
    Task<IEnumerable<Meal>> GetMealsForDateAsync(DateTime date);

    /// <summary>
    /// Calculates daily totals for nutritional metrics.
    /// </summary>
    Task<NutritionalMetric> GetDailyTotalsAsync(DateTime date);

    /// <summary>
    /// Gets daily nutritional summary as a string for UI binding.
    /// </summary>
    Task<string> GetDailySummaryAsync(DateTime date);
}
