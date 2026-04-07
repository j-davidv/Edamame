# Meal Tracker - Quick Reference

## Project Overview

A professional Windows Forms desktop application for meal tracking and nutritional analysis using:
- **Database:** LiteDB (NoSQL)
- **AI:** Google Gemini API
- **Architecture:** Clean Architecture with SOLID principles
- **.NET:** v10

## Directory Structure

```
Domain/              # Business rules & entities
├─ Entities/        # NutritionalBase, Meal, Recipe, Ingredient
└─ Interfaces/      # IRepository, INutritionAnalysisService, IDailyMealAggregator

Application/        # Use cases & orchestration
└─ Services/        # MealService, DailyMealAggregator

Infrastructure/     # Technical implementations
├─ Persistence/     # LiteDbRepository, LiteDbConnectionFactory
├─ ExternalServices/# GeminiNutritionAnalysisService
└─ Configuration/   # ServiceCollectionExtensions (DI setup)

UI/                 # Presentation layer
├─ Form1.cs         # Main window
├─ Form1.Designer.cs
└─ Form1.resx

Program.cs          # Entry point with DI container
```

## Quick Setup (5 minutes)

### 1. Get API Key
- Visit https://ai.google.dev
- Click "Get API Key"
- Copy the key (starts with `AIza`)

### 2. Set Environment Variable
**Windows:** 
```cmd
setx GEMINI_API_KEY "AIza_your_key_here"
```

**Mac/Linux:**
```bash
export GEMINI_API_KEY="AIza_your_key_here"
```

### 3. Run
```bash
dotnet run
```

## Code Patterns

### Create & Analyze a Meal
```csharp
var mealService = serviceProvider.GetRequiredService<MealService>();

var meal = new Meal
{
    Name = "Lunch",
    Type = MealType.Lunch,
    Recipes = new()
    {
        new Recipe
        {
            Name = "Salad",
            Ingredients = new()
            {
                new() { Name = "Lettuce", Quantity = 100, Unit = "grams" }
            }
        }
    }
};

var analyzed = await mealService.CreateAndAnalyzeMealAsync(meal);
Console.WriteLine($"Calories: {analyzed.Nutritionals?.Calories}");
```

### Get Daily Summary
```csharp
string summary = await mealService.GetDailySummaryAsync(DateTime.Today);
Console.WriteLine(summary);
```

### Query Meals
```csharp
var todaysMeals = await mealService.GetMealsForDateAsync(DateTime.Today);
foreach (var meal in todaysMeals)
{
    Console.WriteLine($"{meal.Name}: {meal.Nutritionals?.Calories} kcal");
}
```

## Architecture Decisions

| Decision | Reason |
|----------|--------|
| Clean Architecture | Separation of concerns, testability |
| SOLID Principles | Maintainability, extensibility |
| DI Container | Loose coupling, easy testing |
| Thread-Safe Repository | Safe concurrent access |
| JSON Schema | Consistent API responses |
| LiteDB | Simple local persistence, no setup |
| Gemini API | Free tier, good quality, fast |

## Key Classes

### Domain Layer
- `NutritionalBase` - Base for nutritional data (100 calories, 5g protein, etc.)
- `Meal` - Container for recipes with nutritional analysis
- `Recipe` - Container for ingredients
- `Ingredient` - Individual food items with quantity

### Application Layer
- `MealService` - Orchestrates meal creation, analysis, retrieval
- `DailyMealAggregator` - Calculates daily nutritional totals

### Infrastructure Layer
- `LiteDbRepository<T>` - Generic CRUD operations (thread-safe)
- `GeminiNutritionAnalysisService` - AI-powered analysis
- `LiteDbConnectionFactory` - Database connection management

## SOLID Principles

### Single Responsibility ✅
- `MealService` handles meal workflows only
- `DailyMealAggregator` handles aggregation only
- `LiteDbRepository<T>` handles CRUD only

### Open/Closed ✅
- Open for new analytics services (implement `INutritionAnalysisService`)
- Closed for modification (interfaces define contracts)

### Liskov Substitution ✅
- Any `IRepository<T>` works the same way
- Can swap `LiteDbRepository<T>` with `SqlRepository<T>` without changes

### Interface Segregation ✅
- `IRepository<T>` has exactly 5 methods (Get, GetAll, Create, Update, Delete)
- `INutritionAnalysisService` has exactly 3 methods

### Dependency Inversion ✅
- `MealService` depends on interfaces, not concrete classes
- DI container manages dependencies

## Error Handling

```csharp
try
{
    var result = await mealService.CreateAndAnalyzeMealAsync(meal);
}
catch (ArgumentNullException ex)
{
    // Invalid input
    MessageBox.Show("Please fill all required fields");
}
catch (InvalidOperationException ex)
{
    // API response invalid
    MessageBox.Show($"API error: {ex.Message}");
}
catch (HttpRequestException ex)
{
    // Network error
    MessageBox.Show("Network error. Check your connection");
}
catch (Exception ex)
{
    // Unexpected error
    MessageBox.Show($"Unexpected error: {ex.Message}");
}
```

## Database

### Automatic Setup
- Created in: `%APPDATA%\MealTracker\meals.db`
- BsonMapper configured for nested objects
- Thread-safe operations with ReaderWriterLockSlim

### Collections
- `Meal` - Stores meals with nutritional data
- `Recipe` - Stores recipe templates
- `Ingredient` - Individual ingredients

## API Integration

### Prompt Format
```
Analyze the nutritional composition of the following meal and respond ONLY with valid JSON:

{
    "nutritional_metrics": {
        "calories": 450,
        "protein": 25.5,
        ...
    },
    "dietary_classification": "Vegan",
    "dietary_advice": "..."
}
```

### Response Parsing
```csharp
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var parsed = JsonSerializer.Deserialize<GeminiNutritionResponse>(jsonText, options);
```

## Testing Strategy

```csharp
// Mock repository
var mockRepo = new Mock<IRepository<Meal>>();
mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { meal });

// Mock service
var mockNutrition = new Mock<INutritionAnalysisService>();

// Inject and test
var service = new MealService(mockRepo.Object, mockNutrition.Object, ...);
```

## Performance Tips

1. **Caching:** Cache daily summaries
2. **Batching:** Analyze multiple meals together
3. **Async:** All I/O is async
4. **Lazy Loading:** Load data only when needed

## Troubleshooting

| Problem | Solution |
|---------|----------|
| API key not found | Set `GEMINI_API_KEY` environment variable |
| Database permission denied | Check write access to `%APPDATA%\MealTracker` |
| JSON parsing error | Ensure API response matches schema |
| Connection timeout | Check network, increase timeout |
| Thread timeout | Too many concurrent operations |

## Files You Need to Know

- `Program.cs` - Entry point, DI container setup
- `Form1.cs` - Main UI, service injection
- `MealService.cs` - Business logic entry point
- `LiteDbRepository.cs` - Database operations
- `GeminiNutritionAnalysisService.cs` - AI integration
- `ServiceCollectionExtensions.cs` - DI configuration

## Common Tasks

### Add a new property to Meal
1. Add property to `Meal` entity
2. Update `MealService` if needed
3. Update UI form if needed

### Change nutrition analysis logic
1. Implement new `INutritionAnalysisService`
2. Register in `ServiceCollectionExtensions`
3. Inject into `MealService`

### Add database persistence
1. Create repository extending `LiteDbRepository<T>`
2. Register in DI container
3. Inject via constructor

## Resources

- [Full Architecture Docs](ARCHITECTURE.md)
- [Setup Guide](SETUP.md)
- [Configuration Guide](CONFIG.md)
- [Code Examples](Examples/UsageExamples.cs)
- [LiteDB Docs](https://www.litedb.org)
- [Gemini API Docs](https://ai.google.dev/docs)

## Next Steps

1. ✅ Project structure created
2. ✅ Core models implemented
3. ✅ Service layer created
4. ✅ Database layer created
5. ✅ DI configured
6. **→ Build your UI components**
7. **→ Test with sample data**
8. **→ Deploy to production**

---

**Questions?** See the full documentation in `ARCHITECTURE.md` and `SETUP.md`
