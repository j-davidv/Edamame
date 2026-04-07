using Edamam.Domain.Entities;

namespace Edamam.Domain.Interfaces;

/// abstraction for daily meal aggregation

public interface IDailyMealAggregator
{
    Task<IEnumerable<Meal>> GetMealsForDateAsync(DateTime date);

    Task<NutritionalMetric> GetDailyTotalsAsync(DateTime date);

    /// gets daily nutritional summary as a string for UI binding
    Task<string> GetDailySummaryAsync(DateTime date);
}
