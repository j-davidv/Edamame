# Quick Reference Card - Gemini API Migration

## 🎯 What You Need to Know

### Before Running
1. Get API key from [Google AI Studio](https://aistudio.google.com/apikey)
2. Set environment variable: `GEMINI_API_KEY=your-key`
3. Build solution: `dotnet build`

### What Changed
- ❌ EDAMAM API removed
- ✅ Gemini 2.5 Flash API added
- ✅ Chat service added
- ✅ No database changes

---

## 📋 Service Interfaces

### Nutrition Analysis
```csharp
interface INutritionAnalysisService
{
    Task<NutritionalMetric> AnalyzeMealAsync(Meal meal);
    Task<NutritionalMetric> AnalyzeRecipeAsync(Recipe recipe);
    Task<string> GetDietaryAdviceAsync(Meal meal);
}
```

### Chat Service (New)
```csharp
interface IGeminiChatService
{
    Task<string> ChatAsync(string userMessage);
    Task<string> ChatWithContextAsync(string userMessage, string? context);
    Task ClearHistoryAsync();
}
```

---

## 💻 Code Snippets

### Analyze a Meal
```csharp
[HttpPost("analyze")]
public async Task<IActionResult> AnalyzeMeal(Meal meal)
{
    var result = await _nutritionService.AnalyzeMealAsync(meal);
    await _mealRepository.CreateAsync(meal);
    return Ok(result);
}
```

### Ask AI a Question
```csharp
[HttpPost("chat")]
public async Task<IActionResult> Chat(string question)
{
    var response = await _chatService.ChatAsync(question);
    return Ok(new { response });
}
```

### Chat with Meal Context
```csharp
[HttpPost("chat-with-meal")]
public async Task<IActionResult> ChatAboutMeal(string question, Meal meal)
{
    var context = $"Meal: {meal.Name}, Calories: {meal.Nutritionals?.Calories}";
    var response = await _chatService.ChatWithContextAsync(question, context);
    return Ok(new { response });
}
```

---

## 🔧 DI Setup

### In Program.cs
```csharp
builder.Services.AddMealTrackerServices();

var app = builder.Build();
app.Run();
```

### Inject Services
```csharp
public class MealController : ControllerBase
{
    private readonly INutritionAnalysisService _nutrition;
    private readonly IGeminiChatService _chat;
    
    public MealController(
        INutritionAnalysisService nutrition,
        IGeminiChatService chat)
    {
        _nutrition = nutrition;
        _chat = chat;
    }
}
```

---

## 🗄️ Database (No Changes)

### Save Analyzed Meal
```csharp
var meal = new Meal { /* ... */ };
meal.Nutritionals = await _nutrition.AnalyzeMealAsync(meal);
await _repository.CreateAsync(meal);  // ✅ Works with LiteDB
```

### Retrieve Analyzed Meal
```csharp
var meal = await _repository.GetByIdAsync(mealId);
// meal.Nutritionals.Calories ✅ Available
// meal.Recipes[0].Ingredients ✅ All nested objects intact
```

---

## 🐛 Debugging

### Check API Key
```powershell
$env:GEMINI_API_KEY
# Should output your API key
```

### View Debug Output
1. Visual Studio → Debug → Windows → Output
2. Select "Debug" from dropdown
3. Look for "Gemini Request:" and "Gemini Response:" lines

### Run Debug Program
```powershell
dotnet run --project Program.Debug.cs
```

---

## 🚀 Testing

### Test Nutrition Analysis
```csharp
[Test]
public async Task TestGeminiAnalysis()
{
    var service = new GeminiNutritionAnalysisService();
    var meal = new Meal
    {
        Recipes = new List<Recipe>
        {
            new Recipe
            {
                Ingredients = new List<Ingredient>
                {
                    new Ingredient { Name = "chicken", Quantity = 200, Unit = "gram" }
                }
            }
        }
    };
    
    var result = await service.AnalyzeMealAsync(meal);
    Assert.Greater(result.Calories, 0);
}
```

### Test Chat Service
```csharp
[Test]
public async Task TestChat()
{
    var service = new GeminiChatService();
    var response = await service.ChatAsync("What is protein?");
    Assert.IsNotEmpty(response);
}
```

---

## 📊 Nutrition Metrics

Returns from `AnalyzeMealAsync()`:
- **Calories** (decimal) - kcal
- **Protein** (decimal) - grams
- **Carbohydrates** (decimal) - grams
- **Fat** (decimal) - grams
- **Sodium** (decimal) - mg
- **Sugar** (decimal) - grams
- **SaturatedFat** (decimal) - grams
- **DietaryClassification** (string) - e.g., "High Protein"
- **DietaryAdvice** (string) - e.g., "Great for muscle recovery"

---

## ⚡ Common Errors & Fixes

| Error | Fix |
|-------|-----|
| "GEMINI_API_KEY is required" | Set environment variable |
| "Invalid API key" | Get new key from AI Studio |
| "No response from API" | Check internet connection |
| "Empty response" | Verify valid ingredients provided |

---

## 📚 Documentation Files

| File | Purpose |
|------|---------|
| `GEMINI_SETUP_GUIDE.md` | Complete setup & config |
| `GEMINI_CHAT_SERVICE_GUIDE.md` | Chat service examples |
| `GEMINI_MIGRATION_COMPLETE.md` | Detailed migration info |
| `LITEDB_COMPATIBILITY_VERIFICATION.md` | Database compatibility |
| `GEMINI_MIGRATION_SUMMARY.md` | Executive summary |

---

## ✅ Verification

After setup, verify with:
```csharp
var nutrition = new GeminiNutritionAnalysisService();
var chat = new GeminiChatService();

// Test nutrition
var meal = CreateTestMeal();
var analysis = await nutrition.AnalyzeMealAsync(meal);
Assert.Greater(analysis.Calories, 0); // ✅ Should pass

// Test chat
var response = await chat.ChatAsync("Hello");
Assert.IsNotEmpty(response); // ✅ Should pass
```

---

## 🎓 Learning Resources

- [Google Gemini Docs](https://ai.google.dev/)
- [API Documentation](https://ai.google.dev/api)
- [Google AI Studio](https://aistudio.google.com)
- [LiteDB Docs](https://www.litedb.org/)

---

## 📝 Files Modified

✅ `EdamamNutritionAnalysisService.cs` - Replaced with GeminiNutritionAnalysisService
✅ `ServiceCollectionExtensions.cs` - Updated DI
✅ `Program.Debug.cs` - Updated refs
✅ `Program.Debug.Advanced.cs` - Updated refs
✅ `IGeminiChatService.cs` - Created
✅ `GeminiChatService.cs` - Created

---

## 🔐 Security Notes

1. **Never commit API keys** to git
2. Use environment variables or secrets manager
3. API key is read-only once created
4. Can revoke keys anytime in AI Studio
5. No sensitive data stored in responses

---

## 📞 Support Checklist

- [ ] Gemini API key obtained
- [ ] Environment variable set
- [ ] Build successful
- [ ] Nutrition analysis works
- [ ] Chat service responds
- [ ] Debug output shows Gemini calls
- [ ] Database saves/retrieves data
- [ ] No EDAMAM references in code

---

## 🚀 Ready to Go!

Once you:
1. ✅ Set GEMINI_API_KEY environment variable
2. ✅ Build the solution
3. ✅ Run the application

You can immediately start using:
- **Nutrition analysis** - Same as before, but better
- **Meal advice** - AI-powered recommendations
- **Chat service** - New! Ask nutrition questions

---

**Last Updated:** 2024
**Status:** ✅ Ready for Production
