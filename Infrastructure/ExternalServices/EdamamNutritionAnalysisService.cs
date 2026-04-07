using System.Text.Json;
using System.Text.Json.Serialization;
using Google.GenAI;
using Google.GenAI.Types;
using TEST.Domain.Entities;
using TEST.Domain.Interfaces;

namespace TEST.Infrastructure.ExternalServices;

/// <summary>
/// Implements INutritionAnalysisService using Gemini 2.5 Flash API.
/// Provides accurate nutritional analysis with AI-powered ingredient parsing and evaluation.
/// </summary>
public class GeminiNutritionAnalysisService : INutritionAnalysisService
{
    private readonly Client _geminiClient;
    private const string MODEL_NAME = "gemini-2.5-flash";

    public GeminiNutritionAnalysisService(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key cannot be null or empty.", nameof(apiKey));

        _geminiClient = new Client(apiKey: apiKey);
    }

    /// <summary>
    /// Analyzes a meal using Gemini API.
    /// </summary>
    public async Task<NutritionalMetric> AnalyzeMealAsync(Meal meal)
    {
        if (meal == null) throw new ArgumentNullException(nameof(meal));

        // Defensive check: ensure Recipes is not null
        if (meal.Recipes == null)
        {
            meal.Recipes = new List<Recipe>();
        }

        // Combine all ingredients from recipes
        var allIngredients = meal.Recipes
            .Where(r => r != null && r.Ingredients != null)
            .SelectMany(r => r.Ingredients)
            .Where(i => i != null && !string.IsNullOrWhiteSpace(i.Name))
            .Select(i => $"{i.Quantity} {i.Unit} {i.Name}")
            .ToList();

        if (allIngredients.Count == 0)
        {
            throw new InvalidOperationException($"Meal '{meal.Name}' has no ingredients to analyze. Please provide at least one ingredient.");
        }

        try
        {
            var nutritionalData = await CallGeminiApiAsync(allIngredients);

            return new NutritionalMetric
            {
                MealName = meal.Name,
                Calories = (decimal)nutritionalData.Calories,
                Protein = (decimal)nutritionalData.Protein,
                Carbohydrates = (decimal)nutritionalData.Carbohydrates,
                Fat = (decimal)nutritionalData.Fat,
                Sodium = (decimal)nutritionalData.Sodium,
                Sugar = (decimal)nutritionalData.Sugar,
                SaturatedFat = (decimal)nutritionalData.SaturatedFat,
                DietaryClassification = DetermineDietaryClassification(nutritionalData),
                DietaryAdvice = GenerateDietaryAdvice(nutritionalData),
                AnalyzedDate = DateTime.UtcNow
            };
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"HTTP error calling Gemini API: {ex.Message}", ex);
        }
        catch (OperationCanceledException ex)
        {
            throw new InvalidOperationException("Gemini API request timed out. Check your internet connection.", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error analyzing meal: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Analyzes a recipe using Gemini API.
    /// </summary>
    public async Task<NutritionalMetric> AnalyzeRecipeAsync(Recipe recipe)
    {
        if (recipe == null) throw new ArgumentNullException(nameof(recipe));

        if (recipe.Ingredients.Count == 0)
        {
            throw new InvalidOperationException("Recipe has no ingredients to analyze.");
        }

        try
        {
            var ingredients = recipe.Ingredients
                .Select(i => $"{i.Quantity} {i.Unit} {i.Name}")
                .ToList();

            var nutritionalData = await CallGeminiApiAsync(ingredients);

            return new NutritionalMetric
            {
                MealName = recipe.Name,
                Calories = (decimal)nutritionalData.Calories,
                Protein = (decimal)nutritionalData.Protein,
                Carbohydrates = (decimal)nutritionalData.Carbohydrates,
                Fat = (decimal)nutritionalData.Fat,
                Sodium = (decimal)nutritionalData.Sodium,
                Sugar = (decimal)nutritionalData.Sugar,
                SaturatedFat = (decimal)nutritionalData.SaturatedFat,
                DietaryClassification = DetermineDietaryClassification(nutritionalData),
                DietaryAdvice = GenerateDietaryAdvice(nutritionalData),
                AnalyzedDate = DateTime.UtcNow
            };
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"HTTP error calling Gemini API: {ex.Message}", ex);
        }
        catch (OperationCanceledException ex)
        {
            throw new InvalidOperationException("Gemini API request timed out. Check your internet connection.", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error analyzing recipe: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets dietary advice for a meal using calculated nutritional data.
    /// </summary>
    public async Task<string> GetDietaryAdviceAsync(Meal meal)
    {
        if (meal == null) throw new ArgumentNullException(nameof(meal));

        if (meal.Nutritionals == null)
        {
            return "No nutritional data available. Analyze the meal first.";
        }

        // Analyze nutritional composition and provide advice
        var advice = new List<string>();

        if (meal.Nutritionals.Calories > 2500)
            advice.Add("This meal is calorie-dense. Consider portion control.");
        else if (meal.Nutritionals.Calories < 300)
            advice.Add("This is a light meal. Great for snacks or side dishes.");

        if (meal.Nutritionals.Protein < 10)
            advice.Add("Consider adding more protein sources for better satiety.");
        else if (meal.Nutritionals.Protein > 50)
            advice.Add("This meal has excellent protein content for muscle support.");

        if (meal.Nutritionals.SaturatedFat > 15)
            advice.Add("Reduce saturated fat intake by choosing leaner options.");

        if (meal.Nutritionals.Sodium > 2000)
            advice.Add("Monitor sodium levels; this meal exceeds daily recommendations.");

        if (advice.Count == 0)
            advice.Add("This meal has balanced nutritional composition.");

        return string.Join(" ", advice);
    }

    /// <summary>
    /// Calls Gemini API with ingredient list and structured JSON response.
    /// Uses strict JSON schema to ensure machine-readable responses.
    /// </summary>
    private async Task<GeminiNutritionData> CallGeminiApiAsync(List<string> ingredients)
    {
        try
        {
            var ingredientsList = string.Join("\n", ingredients);

            var prompt = $@"Analyze the following ingredients and calculate their nutritional values per serving or per combined meal.
Ingredients:
{ingredientsList}

Return ONLY a valid JSON object with no additional text or formatting (no markdown, no code blocks) containing exactly these fields:
{{
  ""calories"": <number - total calories>,
  ""protein"": <number - grams of protein>,
  ""carbohydrates"": <number - grams of total carbohydrates>,
  ""fat"": <number - grams of total fat>,
  ""sodium"": <number - milligrams of sodium>,
  ""sugar"": <number - grams of total sugar>,
  ""saturatedFat"": <number - grams of saturated fat>
}}

Ensure all values are numbers and non-negative. If a value cannot be determined, use 0.";

            System.Diagnostics.Debug.WriteLine($"Gemini Request: {prompt}");

            var response = await _geminiClient.Models.GenerateContentAsync(
                model: MODEL_NAME,
                contents: prompt
            );

            if (response?.Candidates == null || response.Candidates.Count == 0)
            {
                throw new InvalidOperationException("Gemini API returned no candidates in response.");
            }

            var candidate = response.Candidates[0];
            if (candidate?.Content?.Parts == null || candidate.Content.Parts.Count == 0)
            {
                throw new InvalidOperationException("Gemini API returned no content parts in response.");
            }

            var responseText = candidate.Content.Parts[0].Text;
            if (string.IsNullOrWhiteSpace(responseText))
            {
                throw new InvalidOperationException("Gemini API returned empty response text.");
            }

            System.Diagnostics.Debug.WriteLine($"Gemini Response: {responseText}");

            // Clean the response (remove markdown code blocks if present)
            var cleanedResponse = CleanJsonResponse(responseText);

            // Parse JSON response using System.Text.Json with case-insensitive matching
            using (var doc = JsonDocument.Parse(cleanedResponse))
            {
                var root = doc.RootElement;

                var nutritionData = new GeminiNutritionData
                {
                    Calories = ExtractNumericValue(root, "calories"),
                    Protein = ExtractNumericValue(root, "protein"),
                    Carbohydrates = ExtractNumericValue(root, "carbohydrates"),
                    Fat = ExtractNumericValue(root, "fat"),
                    Sodium = ExtractNumericValue(root, "sodium"),
                    Sugar = ExtractNumericValue(root, "sugar"),
                    SaturatedFat = ExtractNumericValue(root, "saturatedFat")
                };

                if (nutritionData.Calories == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"WARNING: No calorie data found in Gemini response. Response: {cleanedResponse}");
                    throw new InvalidOperationException($"Gemini API response missing calorie data. Please check your ingredients are valid (e.g., '200 gram chicken breast').");
                }

                return nutritionData;
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Error parsing Gemini JSON response: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error calling Gemini API: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Cleans JSON response by removing markdown code blocks if present.
    /// </summary>
    private static string CleanJsonResponse(string response)
    {
        response = response.Trim();

        // Remove markdown code blocks
        if (response.StartsWith("```"))
        {
            // Remove opening ```json or ```
            var startIndex = response.IndexOf('\n');
            if (startIndex > 0)
            {
                response = response.Substring(startIndex + 1);
            }
            else
            {
                response = response.Substring(3);
            }
        }

        if (response.EndsWith("```"))
        {
            response = response.Substring(0, response.Length - 3);
        }

        return response.Trim();
    }

    /// <summary>
    /// Extracts numeric value from JSON element by property name (case-insensitive).
    /// </summary>
    private static double ExtractNumericValue(JsonElement root, string propertyName)
    {
        // Try exact match first
        if (root.TryGetProperty(propertyName, out var element))
        {
            return ExtractDouble(element);
        }

        // Try case-insensitive match
        foreach (var property in root.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ExtractDouble(property.Value);
            }
        }

        return 0;
    }

    /// <summary>
    /// Safely extracts double value from JsonElement.
    /// </summary>
    private static double ExtractDouble(JsonElement element)
    {
        try
        {
            if (element.ValueKind == JsonValueKind.Number)
            {
                return element.GetDouble();
            }
            else if (element.ValueKind == JsonValueKind.String)
            {
                var str = element.GetString();
                if (double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
                {
                    return result;
                }
            }
        }
        catch
        {
            // Fall through to return 0
        }

        return 0;
    }

    /// <summary>
    /// Determines dietary classification based on nutritional data.
    /// </summary>
    private static string DetermineDietaryClassification(GeminiNutritionData data)
    {
        var classifications = new List<string>();

        // Protein-rich
        if (data.Protein > 30)
            classifications.Add("High Protein");

        // Low calorie
        if (data.Calories < 400)
            classifications.Add("Low Calorie");

        // Low fat
        if (data.Fat < 10)
            classifications.Add("Low Fat");

        // High carb
        if (data.Carbohydrates > 40)
            classifications.Add("High Carb");

        // Balanced
        if (classifications.Count == 0)
            classifications.Add("Balanced");

        return string.Join(", ", classifications);
    }

    /// <summary>
    /// Generates simple dietary advice based on nutritional profile.
    /// </summary>
    private static string GenerateDietaryAdvice(GeminiNutritionData data)
    {
        if (data.Calories > 2000)
            return "High calorie meal - suitable for post-workout recovery or active individuals.";

        if (data.Protein > 40)
            return "Excellent protein content - great for muscle recovery and satiety.";

        if (data.Fat < 5)
            return "Very lean meal - ideal for weight management.";

        if (data.Sodium > 1500)
            return "Consider this sodium-rich meal as occasional rather than daily.";

        return "Well-balanced meal suitable for most dietary goals.";
    }
}

/// <summary>
/// Data structure for processed Gemini nutrition data.
/// </summary>
internal class GeminiNutritionData
{
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Carbohydrates { get; set; }
    public double Fat { get; set; }
    public double Sodium { get; set; }
    public double Sugar { get; set; }
    public double SaturatedFat { get; set; }
}
