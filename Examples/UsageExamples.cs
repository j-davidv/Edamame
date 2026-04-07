// Example: Configuration and Usage Guide
// This file demonstrates how to use the Meal Tracker architecture

using Microsoft.Extensions.DependencyInjection;
using TEST.Application.Services;
using TEST.Domain.Entities;
using TEST.Infrastructure.Configuration;

namespace TEST.Examples;

/// <summary>
/// Usage examples for the Meal Tracker application.
/// Copy these patterns into your UI code.
/// </summary>
public static class UsageExamples
{
    /// <summary>
    /// Example 1: Initialize the application with DI container
    /// </summary>
    public static IServiceProvider SetupDependencyInjection()
    {
        var services = new ServiceCollection();

        // Get API key from environment variable
        string? geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

        // Register all services
        services.AddMealTrackerServices(geminiApiKey);

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Example 2: Create a new meal and analyze it
    /// </summary>
    public static async Task CreateAndAnalyzeMealExample(IServiceProvider serviceProvider)
    {
        var mealService = serviceProvider.GetRequiredService<MealService>();

        var meal = new Meal
        {
            Name = "Healthy Lunch",
            Notes = "Mediterranean style",
            Type = MealType.Lunch,
            MealDate = DateTime.UtcNow,
            Recipes = new()
            {
                new Recipe
                {
                    Name = "Grilled Salmon",
                    Description = "Fresh Atlantic salmon",
                    Ingredients = new()
                    {
                        new() { Name = "Salmon fillet", Quantity = 150, Unit = "grams" },
                        new() { Name = "Lemon", Quantity = 1, Unit = "medium" },
                        new() { Name = "Olive oil", Quantity = 1, Unit = "tablespoon" }
                    }
                },
                new Recipe
                {
                    Name = "Roasted Vegetables",
                    Description = "Mixed seasonal vegetables",
                    Ingredients = new()
                    {
                        new() { Name = "Broccoli", Quantity = 200, Unit = "grams" },
                        new() { Name = "Carrot", Quantity = 150, Unit = "grams" },
                        new() { Name = "Olive oil", Quantity = 1, Unit = "tablespoon" }
                    }
                }
            }
        };

        try
        {
            var analyzedMeal = await mealService.CreateAndAnalyzeMealAsync(meal);
            
            Console.WriteLine($"✅ Meal created: {analyzedMeal.Name}");
            Console.WriteLine($"📊 Nutritional Data:");
            Console.WriteLine($"  - Calories: {analyzedMeal.Nutritionals?.Calories:F0} kcal");
            Console.WriteLine($"  - Protein: {analyzedMeal.Nutritionals?.Protein:F1}g");
            Console.WriteLine($"  - Classification: {analyzedMeal.Nutritionals?.DietaryClassification}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Example 3: Get daily nutritional summary
    /// </summary>
    public static async Task GetDailySummaryExample(IServiceProvider serviceProvider)
    {
        var mealService = serviceProvider.GetRequiredService<MealService>();

        try
        {
            string summary = await mealService.GetDailySummaryAsync(DateTime.Today);
            Console.WriteLine(summary);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading summary: {ex.Message}");
        }
    }

    /// <summary>
    /// Example 4: Query meals by date
    /// </summary>
    public static async Task QueryMealsByDateExample(IServiceProvider serviceProvider)
    {
        var mealService = serviceProvider.GetRequiredService<MealService>();

        try
        {
            var todaysMeals = await mealService.GetMealsForDateAsync(DateTime.Today);

            Console.WriteLine($"📅 Meals for {DateTime.Today:yyyy-MM-dd}:");
            foreach (var meal in todaysMeals)
            {
                Console.WriteLine($"  • {meal.Name} ({meal.Type}) - {meal.Nutritionals?.Calories:F0} kcal");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Example 5: Create a recipe template
    /// </summary>
    public static async Task CreateRecipeTemplateExample(IServiceProvider serviceProvider)
    {
        var mealService = serviceProvider.GetRequiredService<MealService>();

        var recipe = new Recipe
        {
            Name = "Greek Salad",
            Description = "Traditional Greek salad with feta",
            Ingredients = new()
            {
                new() { Name = "Cucumber", Quantity = 200, Unit = "grams" },
                new() { Name = "Tomato", Quantity = 250, Unit = "grams" },
                new() { Name = "Feta cheese", Quantity = 100, Unit = "grams" },
                new() { Name = "Olives", Quantity = 50, Unit = "grams" },
                new() { Name = "Olive oil", Quantity = 2, Unit = "tablespoons" }
            }
        };

        try
        {
            var savedRecipe = await mealService.CreateAndAnalyzeRecipeAsync(recipe);
            Console.WriteLine($"✅ Recipe saved: {savedRecipe.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Example 6: Retrieve all recipes
    /// </summary>
    public static async Task GetAllRecipesExample(IServiceProvider serviceProvider)
    {
        var mealService = serviceProvider.GetRequiredService<MealService>();

        try
        {
            var recipes = await mealService.GetAllRecipesAsync();
            
            Console.WriteLine("📚 Available Recipes:");
            foreach (var recipe in recipes)
            {
                Console.WriteLine($"  • {recipe.Name} - {recipe.Ingredients.Count} ingredients");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Example 7: Update an existing meal
    /// </summary>
    public static async Task UpdateMealExample(IServiceProvider serviceProvider, string mealId)
    {
        var mealService = serviceProvider.GetRequiredService<MealService>();

        try
        {
            var meal = await mealService.GetMealByIdAsync(mealId);
            if (meal != null)
            {
                meal.Notes = "Updated notes";
                bool updated = await mealService.UpdateMealAsync(mealId, meal);
                
                if (updated)
                    Console.WriteLine($"✅ Meal updated: {meal.Name}");
                else
                    Console.WriteLine($"❌ Failed to update meal");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Example 8: Delete a meal
    /// </summary>
    public static async Task DeleteMealExample(IServiceProvider serviceProvider, string mealId)
    {
        var mealService = serviceProvider.GetRequiredService<MealService>();

        try
        {
            bool deleted = await mealService.DeleteMealAsync(mealId);
            
            if (deleted)
                Console.WriteLine($"✅ Meal deleted");
            else
                Console.WriteLine($"❌ Meal not found");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
}
