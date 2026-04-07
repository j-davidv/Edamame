# Meal Tracker - Setup & Configuration Guide

## Quick Start

### 1. Prerequisites
- .NET 10 SDK installed
- Visual Studio 2022 or later (recommended)
- Gemini API Key (get free tier at: https://ai.google.dev/)

### 2. Get Your Gemini API Key

1. Visit https://ai.google.dev
2. Click **Get API Key** button
3. Create or select a Google Cloud project
4. Copy your API key (it will start with `AIza...`)

### 3. Set Environment Variable (Windows)

**Command Prompt:**
```cmd
set GEMINI_API_KEY=your_api_key_here
```

**PowerShell:**
```powershell
$env:GEMINI_API_KEY = "your_api_key_here"
```

**Permanently (Windows):**
1. Press `Win + X` → Select "System"
2. Click "Advanced system settings"
3. Click "Environment Variables"
4. Add new User variable:
   - Variable name: `GEMINI_API_KEY`
   - Variable value: `your_api_key_here`
5. Click OK and restart Visual Studio

### 4. Run the Application

```bash
dotnet run
```

Or run from Visual Studio: `F5`

## Project Structure

```
TEST/
├── Domain/
│   ├── Entities/
│   │   ├── NutritionalBase.cs      # Base class (INHERITANCE)
│   │   ├── NutritionalMetric.cs    # Nutritional DTO (ENCAPSULATION)
│   │   ├── Ingredient.cs           # Ingredient entity
│   │   ├── Recipe.cs               # Recipe with nested ingredients
│   │   └── Meal.cs                 # Meal with recipes (POLYMORPHISM via enum)
│   └── Interfaces/
│       ├── IRepository.cs          # Generic repo (ABSTRACTION)
│       ├── INutritionAnalysisService.cs  # AI service (ABSTRACTION)
│       └── IDailyMealAggregator.cs       # Aggregation (ABSTRACTION)
│
├── Application/
│   └── Services/
│       ├── MealService.cs          # Orchestration layer
│       └── DailyMealAggregator.cs  # Daily aggregation logic
│
├── Infrastructure/
│   ├── Persistence/
│   │   ├── LiteDbRepository.cs     # Generic repo impl (THREAD-SAFE)
│   │   └── LiteDbConnectionFactory.cs
│   ├── ExternalServices/
│   │   ├── GeminiNutritionAnalysisService.cs  # AI implementation
│   │   └── GeminiNutritionResponse.cs         # DTOs
│   └── Configuration/
│       └── ServiceCollectionExtensions.cs  # DI setup
│
├── UI/
│   ├── Form1.cs           # Main form with DI integration
│   ├── Form1.Designer.cs
│   └── Form1.resx
│
├── Examples/
│   └── UsageExamples.cs   # Code samples
│
├── Program.cs             # Entry point with DI setup
└── ARCHITECTURE.md        # This file

```

## Architecture Highlights

### Clean Architecture Layers

```
┌─────────────────────────────────┐
│   Presentation (UI)             │  Form1.cs, Windows Forms
├─────────────────────────────────┤
│   Application (Business Logic)  │  MealService, DailyMealAggregator
├─────────────────────────────────┤
│   Domain (Core Logic)           │  Entities, Interfaces
├─────────────────────────────────┤
│   Infrastructure (Technical)    │  LiteDB, Gemini API, DI
└─────────────────────────────────┘
```

### SOLID Principles Implementation

| Principle | Implementation |
|-----------|-----------------|
| **Single Responsibility** | Each service has ONE reason to change |
| **Open/Closed** | Open for extension (new analytics), closed for modification |
| **Liskov Substitution** | All repositories implement `IRepository<T>` contract |
| **Interface Segregation** | Small focused interfaces (`IRepository<T>`, `INutritionAnalysisService`) |
| **Dependency Inversion** | High-level modules depend on abstractions, not concretions |

### OOP Principles

| Principle | Example |
|-----------|---------|
| **Inheritance** | `NutritionalMetric : NutritionalBase` |
| **Encapsulation** | Properties with getters/setters, private fields |
| **Polymorphism** | `MealType` enum for different meal processing |
| **Abstraction** | `IRepository<T>`, `INutritionAnalysisService` interfaces |

## Thread Safety

The application uses **ReaderWriterLockSlim** for thread-safe database operations:

```csharp
// In LiteDbRepository<T>
private static readonly ReaderWriterLockSlim _lock = new();

// Read operations
_lock.EnterReadLock();
try { /* read data */ }
finally { _lock.ExitReadLock(); }

// Write operations
_lock.EnterWriteLock();
try { /* write data */ }
finally { _lock.ExitWriteLock(); }
```

**Benefits:**
- Multiple readers can access data simultaneously
- Writers have exclusive access
- No deadlocks
- Async-friendly with `Task.Run()`

## Dependency Injection Setup

The DI container is configured in `ServiceCollectionExtensions.cs`:

```csharp
services.AddMealTrackerServices(geminiApiKey);
```

This registers:
1. **LiteDB database** (Singleton)
2. **Repositories** (Scoped)
3. **Nutrition service** (Scoped)
4. **Business logic services** (Scoped)

## Database

### Location
```
%APPDATA%\MealTracker\meals.db
```

Example: `C:\Users\YourUser\AppData\Roaming\MealTracker\meals.db`

### Collections
- `Meal` - Stores all meals with nutrition data
- `Recipe` - Stores recipe templates
- `Ingredient` - (Optional) Ingredient database

### Custom Database Path

```csharp
services.AddMealTrackerServices(
    geminiApiKey: apiKey,
    databasePath: "C:/CustomPath/meals.db"
);
```

## API Integration

### Gemini API Configuration

The service uses the **Gemini 1.5 Flash** model (fast and free):
- **Endpoint:** `https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent`
- **Rate limits:** Free tier has fair usage limits
- **Response format:** Enforced JSON schema for consistency

### Error Handling

All API calls include comprehensive error handling:

```csharp
try
{
    var nutrition = await _nutritionService.AnalyzeMealAsync(meal);
}
catch (InvalidOperationException ex)
{
    // Handle JSON parsing errors
    MessageBox.Show($"Response format error: {ex.Message}");
}
catch (HttpRequestException ex)
{
    // Handle network/API errors
    MessageBox.Show($"API error: {ex.Message}");
}
catch (Exception ex)
{
    // Handle unexpected errors
    MessageBox.Show($"Error: {ex.Message}");
}
```

## Usage Examples

### Create a Meal

```csharp
var mealService = serviceProvider.GetRequiredService<MealService>();

var meal = new Meal
{
    Name = "Breakfast",
    Type = MealType.Breakfast,
    MealDate = DateTime.UtcNow,
    Recipes = new()
    {
        new Recipe
        {
            Name = "Oatmeal",
            Ingredients = new()
            {
                new() { Name = "Oats", Quantity = 50, Unit = "grams" },
                new() { Name = "Milk", Quantity = 200, Unit = "ml" }
            }
        }
    }
};

var analyzedMeal = await mealService.CreateAndAnalyzeMealAsync(meal);
```

### Get Daily Summary

```csharp
string summary = await mealService.GetDailySummaryAsync(DateTime.Today);
Console.WriteLine(summary);

// Output:
// 📅 Date: 2024-01-15
// 🍽️ Meals: 3
// 
// 📊 Daily Totals:
//   🔥 Calories: 2150 kcal
//   💪 Protein: 95.5g
//   🌾 Carbs: 275.2g
//   🥑 Fat: 52.1g
//   🧂 Sodium: 1850mg
//   🍬 Sugar: 45.3g
//   ⚠️ Saturated Fat: 18.5g
```

### Query by Date

```csharp
var meals = await mealService.GetMealsForDateAsync(DateTime.Today);
foreach (var meal in meals)
{
    Console.WriteLine($"{meal.Name}: {meal.Nutritionals?.Calories} kcal");
}
```

See `Examples/UsageExamples.cs` for more patterns.

## Testing Strategy

The architecture supports unit testing via dependency injection:

```csharp
// Mock the repository
var mockRepository = new Mock<IRepository<Meal>>();
mockRepository
    .Setup(r => r.GetAllAsync())
    .ReturnsAsync(new List<Meal> { /* test data */ });

// Mock the nutrition service
var mockNutritionService = new Mock<INutritionAnalysisService>();

// Inject mocks
var aggregator = new DailyMealAggregator(mockRepository.Object);

// Test
var totals = await aggregator.GetDailyTotalsAsync(DateTime.Today);
Assert.Equal(2500, totals.Calories);
```

## Performance Tips

1. **Caching:** Daily summaries are frequently requested
   ```csharp
   private Dictionary<DateTime, NutritionalMetric> _cache = new();
   ```

2. **Async/Await:** All I/O operations are async
   - Database reads/writes use `Task.Run()`
   - API calls use `HttpClient`

3. **Lazy Loading:** Load recipes only when needed
   ```csharp
   var meals = await mealRepository.GetAllAsync();
   var recipes = await recipeRepository.GetAllAsync();
   ```

## Troubleshooting

### API Key Not Found
```
Error: Gemini API key is required. Provide it as parameter or set GEMINI_API_KEY environment variable.
```
**Solution:** Set the `GEMINI_API_KEY` environment variable and restart the application.

### Database Permission Denied
```
Error: Failed to create Meal. Access to the path '...\meals.db' is denied.
```
**Solution:** 
- Ensure the `%APPDATA%\MealTracker` folder has write permissions
- Close any other instances of the application

### JSON Deserialization Failed
```
Error: Failed to parse Gemini API response as JSON.
```
**Solution:**
- Check your API key is valid
- Ensure the API endpoint is accessible
- Check network connectivity

### Thread Timeout
```
Error: Operation timed out while acquiring write lock.
```
**Solution:**
- Too many concurrent write operations
- Check for deadlocks in nested calls
- Increase timeout in repository

## Extending the Application

### Add a New Service

1. Create interface in `Domain/Interfaces/`
2. Create implementation in `Infrastructure/ExternalServices/`
3. Register in `ServiceCollectionExtensions`
4. Inject into `MealService` or form

### Add Persistence Layer

1. Create repository class extending `LiteDbRepository<T>`
2. Register in DI container
3. Use via `IRepository<T>` interface

### Add UI Forms

1. Create new Windows Form
2. Inject `IServiceProvider` in constructor
3. Get services: `serviceProvider.GetRequiredService<MealService>()`
4. Implement async event handlers

## Additional Resources

- [LiteDB Documentation](https://www.litedb.org)
- [Google Gemini API Docs](https://ai.google.dev/docs)
- [Microsoft DI Documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

## License

Use for educational and commercial purposes with proper attribution.
