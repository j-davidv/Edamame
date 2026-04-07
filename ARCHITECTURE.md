# Meal Tracker - Architecture Documentation

## Project Structure

```
Solution/
├── Domain/
│   ├── Entities/
│   │   ├── NutritionalBase.cs          # Base class for nutritional data
│   │   ├── NutritionalMetric.cs        # DTO for nutritional info
│   │   ├── Ingredient.cs               # Ingredient entity
│   │   ├── Recipe.cs                   # Recipe entity
│   │   └── Meal.cs                     # Meal entity with MealType enum
│   └── Interfaces/
│       ├── IRepository.cs              # Generic repository interface
│       ├── INutritionAnalysisService.cs # AI service interface
│       └── IDailyMealAggregator.cs     # Daily aggregation interface
│
├── Application/
│   └── Services/
│       ├── MealService.cs              # Business logic orchestration
│       └── DailyMealAggregator.cs      # Daily aggregation implementation
│
├── Infrastructure/
│   ├── Persistence/
│   │   ├── LiteDbRepository.cs         # Generic LiteDB repository (thread-safe)
│   │   └── LiteDbConnectionFactory.cs  # Database connection management
│   ├── ExternalServices/
│   │   ├── GeminiNutritionAnalysisService.cs
│   │   └── GeminiNutritionResponse.cs   # DTOs for API responses
│   └── Configuration/
│       └── ServiceCollectionExtensions.cs # DI configuration
│
├── UI/
│   ├── Form1.cs                        # Main form
│   ├── Form1.Designer.cs
│   └── Form1.resx
│
├── Program.cs                          # Entry point with DI setup
└── TEST.csproj                         # Project file with NuGet packages

```

## Architecture Principles

### 1. **Clean Architecture**
- **Domain Layer**: Pure business logic, no dependencies
- **Application Layer**: Use cases and business rules
- **Infrastructure Layer**: Technical implementations (database, APIs)
- **UI Layer**: Presentation and user interaction

### 2. **SOLID Principles**

#### **S - Single Responsibility**
- `LiteDbRepository<T>`: Only handles CRUD operations
- `GeminiNutritionAnalysisService`: Only handles API calls
- `DailyMealAggregator`: Only handles aggregation logic

#### **O - Open/Closed**
- Services are open for extension through inheritance
- Closed for modification via interfaces

#### **L - Liskov Substitution**
- All repository implementations conform to `IRepository<T>`
- All nutrition services implement `INutritionAnalysisService`

#### **I - Interface Segregation**
- Small, focused interfaces (`IRepository<T>`, `INutritionAnalysisService`)
- No fat interfaces with unused methods

#### **D - Dependency Inversion**
- High-level modules depend on abstractions
- Low-level modules implement abstractions
- `MealService` depends on `IRepository<T>`, not concrete repositories

### 3. **Object-Oriented Design**

#### **Inheritance**
- `NutritionalBase`: Base class for all nutritional entities
- Shared validation and properties

#### **Encapsulation**
- Properties with getters/setters in entities
- Private fields in services
- Data isolation through repository pattern

#### **Polymorphism**
- `MealType` enum for polymorphic meal processing
- `IRepository<T>` interface allows different storage implementations

#### **Abstraction**
- `INutritionAnalysisService` abstracts AI implementation
- `IDailyMealAggregator` abstracts aggregation logic
- Concrete implementations hidden behind interfaces

### 4. **Thread Safety**
- **ReaderWriterLockSlim** in `LiteDbRepository<T>`
- **Singleton pattern** with double-check locking for database connection
- **Async/await** for non-blocking operations

## Setup Instructions

### 1. Configure Gemini API Key

Set the environment variable before running:

```bash
# Windows (Command Prompt)
set GEMINI_API_KEY=your_api_key_here

# Windows (PowerShell)
$env:GEMINI_API_KEY = "your_api_key_here"

# Linux/macOS
export GEMINI_API_KEY=your_api_key_here
```

Or in code during DI setup:

```csharp
services.AddMealTrackerServices(geminiApiKey: "your_api_key_here");
```

### 2. Database Location

LiteDB files are automatically created in:
```
%APPDATA%\MealTracker\meals.db
```

Override with custom path:
```csharp
services.AddMealTrackerServices(databasePath: "C:/MyDb/meals.db");
```

## Usage Examples

### Create and Analyze a Meal

```csharp
var mealService = serviceProvider.GetRequiredService<MealService>();

var meal = new Meal
{
    Name = "Breakfast",
    Type = MealType.Breakfast,
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
Console.WriteLine(analyzedMeal.Nutritionals?.ToString());
```

### Get Daily Summary

```csharp
string summary = await mealService.GetDailySummaryAsync(DateTime.Today);
Console.WriteLine(summary);
```

### Query Meals by Date

```csharp
var todaysMeals = await mealService.GetMealsForDateAsync(DateTime.Today);
foreach (var meal in todaysMeals)
{
    Console.WriteLine(meal.ToString());
}
```

## Error Handling

All services implement comprehensive error handling:

- **JSON Deserialization**: Handles invalid Gemini responses
- **API Failures**: Graceful error messages with inner exceptions
- **Database Operations**: Thread-safe error handling with lock management
- **Validation**: Null checks and validation on all inputs

## Testing Considerations

The architecture supports unit testing:

```csharp
// Mock repository
var mockRepository = new Mock<IRepository<Meal>>();

// Mock nutrition service
var mockNutritionService = new Mock<INutritionAnalysisService>();

// Inject mocks into service
var mealService = new MealService(
    mockRepository.Object,
    mockRecipeRepository.Object,
    mockNutritionService.Object,
    mockAggregator.Object
);
```

## Performance Optimizations

1. **Connection Pooling**: LiteDB shared connection
2. **Async Operations**: Non-blocking I/O for database and API
3. **Read/Write Locks**: Prevents concurrent modification issues
4. **Caching**: Can be added to `DailyMealAggregator` for repeated queries

## Security Considerations

1. **API Key Management**: Use environment variables, not hardcoded
2. **Input Validation**: All inputs validated before processing
3. **Database Security**: LiteDB file-based, can be encrypted
4. **Null Safety**: .NET nullable reference types enabled

## Future Enhancements

1. Add caching layer (Redis/Memory)
2. Implement audit logging
3. Add user authentication
4. Create recipe templates library
5. Add export functionality (PDF, CSV)
6. Implement meal recommendations based on history
7. Add barcode scanning for nutrition lookup
8. Multi-user support with sync
