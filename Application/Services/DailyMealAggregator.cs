using Edamam.Domain.Entities;
using Edamam.Domain.Interfaces;

namespace Edamam.Application.Services;


/// <summary>
/// Implements IDailyMealAggregator for daily nutritional computation
/// computes meals from LiteDB and calculates daily totals
/// </summary>


public class DailyMealAggregator : IDailyMealAggregator
{
    private readonly IRepository<Meal> _mealRepository;

    public DailyMealAggregator(IRepository<Meal> mealRepository)
    {
        _mealRepository = mealRepository ?? throw new ArgumentNullException(nameof(mealRepository));
    }

    // Gets all meals for a specific date
    public async Task<IEnumerable<Meal>> GetMealsForDateAsync(DateTime date)
    {
        var allMeals = await _mealRepository.GetAllAsync();
        var targetDate = date.Date;

        return allMeals.Where(m => m.MealDate.Date == targetDate).ToList();
    }

    // Calculates daily totals for all nutritional metrics

    public async Task<NutritionalMetric> GetDailyTotalsAsync(DateTime date)
    {
        var mealsForDay = await GetMealsForDateAsync(date);
        
        if (!mealsForDay.Any())
        {
            return new NutritionalMetric
            {
                MealName = $"Daily Total - {date:yyyy-MM-dd}",
                AnalyzedDate = date
            };
        }

        var totals = new NutritionalMetric
        {
            MealName = $"Daily Total - {date:yyyy-MM-dd}",
            AnalyzedDate = date,
            Calories = 0,
            Protein = 0,
            Carbohydrates = 0,
            Fat = 0,
            Sodium = 0,
            Sugar = 0,
            SaturatedFat = 0
        };

        // compiles nutritional data from all meals
        foreach (var meal in mealsForDay)
        {
            if (meal.Nutritionals != null)
            {
                totals.Calories += meal.Nutritionals.Calories;
                totals.Protein += meal.Nutritionals.Protein;
                totals.Carbohydrates += meal.Nutritionals.Carbohydrates;
                totals.Fat += meal.Nutritionals.Fat;
                totals.Sodium += meal.Nutritionals.Sodium;
                totals.Sugar += meal.Nutritionals.Sugar;
                totals.SaturatedFat += meal.Nutritionals.SaturatedFat;
            }
        }

        return totals;
    }

    public async Task<string> GetDailySummaryAsync(DateTime date)
    {
        var dailyTotals = await GetDailyTotalsAsync(date);
        var mealsCount = (await GetMealsForDateAsync(date)).Count();

        return $@"
📅 Date: {date:yyyy-MM-dd}
🍽️ Meals: {mealsCount}

📊 Daily Totals:
  🔥 Calories: {dailyTotals.Calories:F0} kcal
  💪 Protein: {dailyTotals.Protein:F1}g
  🌾 Carbs: {dailyTotals.Carbohydrates:F1}g
  🥑 Fat: {dailyTotals.Fat:F1}g
  🧂 Sodium: {dailyTotals.Sodium:F0}mg
  🍬 Sugar: {dailyTotals.Sugar:F1}g
  ⚠️ Saturated Fat: {dailyTotals.SaturatedFat:F1}g
";
    }
}
