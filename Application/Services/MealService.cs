using TEST.Domain.Entities;
using TEST.Domain.Interfaces;

namespace TEST.Application.Services;

/// <summary>
/// Provides use-case orchestration for meal operations.
/// Coordinates between repositories and services (Application layer).
/// </summary>
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

    /// <summary>
    /// Creates a new meal and analyzes its nutritional content.
    /// </summary>
    public async Task<Meal> CreateAndAnalyzeMealAsync(Meal meal)
    {
        if (meal == null) throw new ArgumentNullException(nameof(meal));

        // Analyze nutritional content
        meal.Nutritionals = await _nutritionService.AnalyzeMealAsync(meal);

        // Persist to database - LiteDB will automatically assign an ObjectId
        await _mealRepository.CreateAsync(meal);

        return meal;
    }

    /// <summary>
    /// Gets all meals for a specific date.
    /// </summary>
    public async Task<IEnumerable<Meal>> GetMealsForDateAsync(DateTime date)
    {
        return await _dailyAggregator.GetMealsForDateAsync(date);
    }

    /// <summary>
    /// Gets the daily nutritional summary.
    /// </summary>
    public async Task<string> GetDailySummaryAsync(DateTime date)
    {
        return await _dailyAggregator.GetDailySummaryAsync(date);
    }

    /// <summary>
    /// Retrieves a meal by ID.
    /// </summary>
    public async Task<Meal?> GetMealByIdAsync(string id)
    {
        return await _mealRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Updates a meal's nutritional information.
    /// </summary>
    public async Task<bool> UpdateMealAsync(string id, Meal meal)
    {
        return await _mealRepository.UpdateAsync(id, meal);
    }

    /// <summary>
    /// Deletes a meal by ID.
    /// </summary>
    public async Task<bool> DeleteMealAsync(string id)
    {
        return await _mealRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Creates and analyzes a recipe.
    /// </summary>
    public async Task<Recipe> CreateAndAnalyzeRecipeAsync(Recipe recipe)
    {
        if (recipe == null) throw new ArgumentNullException(nameof(recipe));

        // LiteDB will automatically assign an ObjectId during Insert
        await _recipeRepository.CreateAsync(recipe);

        return recipe;
    }

    /// <summary>
    /// Gets all recipes.
    /// </summary>
    public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
    {
        return await _recipeRepository.GetAllAsync();
    }
}
