using Edamam.Domain.Entities;
using Edamam.Domain.Interfaces;

namespace Edamam.Application.Services;

/// coordinates between repos and services (the application layer)

public class MealService
{
    private readonly IRepository<Meal> _mealRepository;
    private readonly IRepository<Recipe> _recipeRepository;
    private readonly INutritionAnalysisService _nutritionService;
    private readonly IDailyMealAggregator _dailyAggregator;

    public MealService(
        IRepository<Meal> mealRepository,
        IRepository<Recipe> recipeRepository,
        INutritionAnalysisService nutritionService,
        IDailyMealAggregator dailyAggregator)
    {
        _mealRepository = mealRepository ?? throw new ArgumentNullException(nameof(mealRepository));
        _recipeRepository = recipeRepository ?? throw new ArgumentNullException(nameof(recipeRepository));
        _nutritionService = nutritionService ?? throw new ArgumentNullException(nameof(nutritionService));
        _dailyAggregator = dailyAggregator ?? throw new ArgumentNullException(nameof(dailyAggregator));
    }


    /// creates a new meal and analyzes its nutritional content

    public async Task<Meal> CreateAndAnalyzeMealAsync(Meal meal)
    {
        if (meal == null) throw new ArgumentNullException(nameof(meal));

        // Analyze nutritional content
        meal.Nutritionals = await _nutritionService.AnalyzeMealAsync(meal);

        // Persist to database - LiteDB will automatically assign an ObjectId
        await _mealRepository.CreateAsync(meal);

        return meal;
    }

    /// sgets all meals for a specific date

    public async Task<IEnumerable<Meal>> GetMealsForDateAsync(DateTime date)
    {
        return await _dailyAggregator.GetMealsForDateAsync(date);
    }

    /// gets the daily nutritional summary
    public async Task<string> GetDailySummaryAsync(DateTime date)
    {
        return await _dailyAggregator.GetDailySummaryAsync(date);
    }

    /// retrieves a meal by ID
    public async Task<Meal?> GetMealByIdAsync(string id)
    {
        return await _mealRepository.GetByIdAsync(id);
    }

    /// updates a meal's nutritional information
    public async Task<bool> UpdateMealAsync(string id, Meal meal)
    {
        return await _mealRepository.UpdateAsync(id, meal);
    }

    /// deletes a meal by ID
    public async Task<bool> DeleteMealAsync(string id)
    {
        return await _mealRepository.DeleteAsync(id);
    }

    /// creates and analyzes a recipe

    public async Task<Recipe> CreateAndAnalyzeRecipeAsync(Recipe recipe)
    {
        if (recipe == null) throw new ArgumentNullException(nameof(recipe));

        // LiteDB will automatically assign an ObjectId during Insert
        await _recipeRepository.CreateAsync(recipe);

        return recipe;
    }

    /// ghets all recipes
    public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
    {
        return await _recipeRepository.GetAllAsync();
    }
}
