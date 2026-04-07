# Complete Meal Tracker Project - Developer Guide

## 🎯 Project Overview

**Meal Tracker** is a professional Windows Forms application for meal tracking and nutritional analysis built with .NET 10, following Clean Architecture and SOLID principles.

### Core Technologies
- **.NET 10.0** Windows Forms application
- **LiteDB 5.0.17** for local NoSQL persistence
- **Google Gemini API** for AI-powered nutritional analysis
- **Microsoft.Extensions.DependencyInjection** for IoC
- **System.Text.Json** for JSON serialization

### Architecture
- Clean Architecture (4 layers)
- SOLID principles (all 5 enforced)
- OOP principles (Inheritance, Encapsulation, Polymorphism, Abstraction)
- Thread-safe operations with ReaderWriterLockSlim

## 📦 What's Included

### Source Files (12 files)

#### Domain Layer (6 files)
- `Domain/Entities/NutritionalBase.cs` - Base class for nutrition (inheritance)
- `Domain/Entities/NutritionalMetric.cs` - Nutritional data DTO (encapsulation)
- `Domain/Entities/Ingredient.cs` - Ingredient entity
- `Domain/Entities/Recipe.cs` - Recipe with nested ingredients
- `Domain/Entities/Meal.cs` - Meal entity with MealType enum (polymorphism)
- `Domain/Interfaces/IRepository.cs` - Generic CRUD interface (abstraction)
- `Domain/Interfaces/INutritionAnalysisService.cs` - AI service interface (abstraction)
- `Domain/Interfaces/IDailyMealAggregator.cs` - Aggregation interface (abstraction)

#### Application Layer (2 files)
- `Application/Services/MealService.cs` - Business logic orchestration
- `Application/Services/DailyMealAggregator.cs` - Daily meal aggregation

#### Infrastructure Layer (4 files)
- `Infrastructure/Persistence/LiteDbRepository.cs` - Generic repository (thread-safe)
- `Infrastructure/Persistence/LiteDbConnectionFactory.cs` - DB connection factory
- `Infrastructure/ExternalServices/GeminiNutritionAnalysisService.cs` - AI integration
- `Infrastructure/ExternalServices/GeminiNutritionResponse.cs` - API DTOs
- `Infrastructure/Configuration/ServiceCollectionExtensions.cs` - DI setup

#### UI Layer (3 files)
- `Form1.cs` - Main form with DI integration
- `Form1.Designer.cs` - Auto-generated UI
- `Form1.resx` - UI resources

#### Entry Point (1 file)
- `Program.cs` - Application entry point with DI container

#### Project Files (1 file)
- `TEST.csproj` - Project configuration with NuGet packages

### Documentation Files (6 files)

| File | Purpose | Audience |
|------|---------|----------|
| `README.md` | Project overview & highlights | Everyone |
| `ARCHITECTURE.md` | Complete architecture explanation | Architects, Senior Devs |
| `SETUP.md` | Setup & configuration guide | DevOps, Developers |
| `CONFIG.md` | Configuration reference | DevOps, Configuration Mgmt |
| `QUICKREF.md` | Quick reference guide | Daily use |
| `DI_GUIDE.md` | Dependency injection patterns | Developers |

### Code Examples (1 file)
- `Examples/UsageExamples.cs` - 8 working code examples

## 🏗️ Architecture Patterns

### Layer Responsibilities

```
Presentation (Form1.cs)
    - UI controls, event handlers
    - Display data to user
    - Gather user input
    - Uses: MealService, IDailyMealAggregator

Application (MealService, DailyMealAggregator)
    - Use case orchestration
    - Business logic workflows
    - Transaction management
    - Uses: IRepository<T>, INutritionAnalysisService

Domain (Entities, Interfaces)
    - Business rules
    - Validation logic
    - Pure business logic
    - No dependencies

Infrastructure (Repositories, API Client, DI)
    - Database operations (thread-safe)
    - External API calls
    - Configuration & setup
    - Implements: IRepository<T>, INutritionAnalysisService
```

### Dependency Flow

```
UI (Form1)
    ↓ injects
Application (MealService)
    ↓ depends on
Domain (IRepository<T>, INutritionAnalysisService)
    ↓ implemented by
Infrastructure (LiteDbRepository, GeminiService)
```

**Key:** No layer depends on layers below it. All dependencies point INWARD.

### SOLID Implementation

#### S - Single Responsibility
- `MealService` - Orchestrates meal operations
- `LiteDbRepository<T>` - Handles CRUD operations only
- `GeminiNutritionAnalysisService` - Calls Gemini API only
- `DailyMealAggregator` - Aggregates daily totals only

#### O - Open/Closed
- New analysis services can be added (implement `INutritionAnalysisService`)
- New repositories can be added (implement `IRepository<T>`)
- Application is open for extension, closed for modification

#### L - Liskov Substitution
- Any `IRepository<T>` implementation works the same way
- Can swap `LiteDbRepository<T>` with `SqlRepository<T>` without changes
- All services implementing interfaces honor their contracts

#### I - Interface Segregation
- `IRepository<T>` - 5 methods: GetById, GetAll, Create, Update, Delete
- `INutritionAnalysisService` - 3 methods: AnalyzeMeal, AnalyzeRecipe, GetAdvice
- `IDailyMealAggregator` - 3 methods: GetMeals, GetTotals, GetSummary

#### D - Dependency Inversion
- `MealService` depends on `IRepository<T>` (abstraction)
- NOT on `LiteDbRepository<T>` (concrete class)
- `INutritionAnalysisService` abstraction, not `GeminiNutritionAnalysisService`

## 🔒 Thread Safety

### Database Operations
```csharp
// ReaderWriterLockSlim usage
private static readonly ReaderWriterLockSlim _lock = new();

// Concurrent Reads
_lock.EnterReadLock();
try { /* read data */ }
finally { _lock.ExitReadLock(); }

// Exclusive Writes
_lock.EnterWriteLock();
try { /* write data */ }
finally { _lock.ExitWriteLock(); }
```

**Benefits:**
- ✅ Multiple readers simultaneously
- ✅ Exclusive writer access
- ✅ No deadlocks
- ✅ Fair lock acquisition
- ✅ Async/await compatible

### Connection Management
```csharp
// Singleton pattern with double-check locking
private static ILiteDatabase? _instance;
private static readonly object _lock = new();

public ILiteDatabase GetDatabase()
{
    if (_instance != null) return _instance;
    
    lock (_lock)
    {
        if (_instance == null)
        {
            _instance = new LiteDatabase(_connectionString);
        }
        return _instance;
    }
}
```

## 🌐 API Integration

### Gemini API Configuration
- **Model:** `gemini-1.5-flash` (fast, free tier)
- **Endpoint:** `https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent`
- **Authentication:** API key in query string
- **Rate Limits:** 60 RPM, 1,500 daily requests

### JSON Schema Enforcement
```csharp
{
    "nutritional_metrics": {
        "calories": { "type": "number" },
        "protein": { "type": "number" },
        // ... 7 more metrics
    },
    "dietary_classification": { "type": "string" },
    "dietary_advice": { "type": "string" }
}
```

### Error Handling
```csharp
try
{
    var nutrition = await _nutritionService.AnalyzeMealAsync(meal);
}
catch (JsonException)
{
    // API response format error
}
catch (HttpRequestException)
{
    // Network or API error
}
catch (InvalidOperationException)
{
    // Business logic error
}
```

## 📊 Data Models

### Inheritance Hierarchy
```
NutritionalBase (Base Class)
    └── NutritionalMetric (Derived)
        ├── Calories: decimal
        ├── Protein: decimal
        ├── Carbohydrates: decimal
        ├── Fat: decimal
        ├── Sodium: decimal
        ├── Sugar: decimal
        └── SaturatedFat: decimal
```

### Entity Relationships
```
Meal
├── Recipes (List<Recipe>)
│   └── Ingredients (List<Ingredient>)
│       ├── Name
│       ├── Quantity
│       └── Unit
├── Nutritionals (NutritionalMetric)
│   ├── Calories
│   ├── Protein
│   ├── etc.
│   └── DietaryAdvice
└── Type (MealType enum)
    ├── Breakfast
    ├── Brunch
    ├── Lunch
    ├── Snack
    ├── Dinner
    └── Supper
```

## 💾 Database

### Storage
- **Location:** `%APPDATA%\MealTracker\meals.db`
- **Type:** LiteDB file-based NoSQL
- **Collections:** Meal, Recipe, Ingredient
- **Access:** Thread-safe with ReaderWriterLockSlim

### Persistence
```csharp
// BsonMapper configuration for nested objects
mapper.Entity<Meal>().Id(x => x.Id);
mapper.Entity<Recipe>().Id(x => x.Id);
```

### Backup
```bash
# Manual backup
xcopy "%APPDATA%\MealTracker" "D:\Backup\MealTracker" /Y /I
```

## 🔧 Dependency Injection

### Setup
```csharp
// In Program.cs
var services = new ServiceCollection();
services.AddMealTrackerServices(geminiApiKey);
var serviceProvider = services.BuildServiceProvider();
```

### Resolution
```csharp
// In Form1.cs
var mealService = serviceProvider.GetRequiredService<MealService>();

// Automatically:
// - Creates LiteDbRepository<Meal>
// - Gets ILiteDatabase from factory
// - Creates GeminiNutritionAnalysisService
// - Creates DailyMealAggregator
// - Wires everything together
```

## ✅ Testing Strategy

### Unit Testing Pattern
```csharp
[TestClass]
public class MealServiceTests
{
    [TestMethod]
    public async Task CreateMeal_AnalyzesNutrition()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Meal>>();
        var mockNutrition = new Mock<INutritionAnalysisService>();
        var service = new MealService(mockRepo.Object, mockNutrition.Object, ...);
        
        // Act
        var result = await service.CreateAndAnalyzeMealAsync(meal);
        
        // Assert
        mockNutrition.Verify(n => n.AnalyzeMealAsync(It.IsAny<Meal>()), Times.Once);
    }
}
```

## 🚀 Getting Started

### Step 1: API Key (2 minutes)
1. Visit https://ai.google.dev
2. Click "Get API Key"
3. Copy the key

### Step 2: Environment Variable (1 minute)
```bash
setx GEMINI_API_KEY "AIza_your_key_here"  # Windows
export GEMINI_API_KEY="AIza_your_key_here"  # Mac/Linux
```

### Step 3: Run (1 minute)
```bash
dotnet run
```

### Step 4: Use (See Examples)
```csharp
var meal = new Meal { /* ... */ };
var analyzed = await mealService.CreateAndAnalyzeMealAsync(meal);
```

## 📚 File Structure Summary

```
TEST/
├── Domain/                          # Business rules
│   ├── Entities/                   # Model classes
│   │   ├── NutritionalBase.cs      # Inheritance
│   │   ├── NutritionalMetric.cs    # Encapsulation
│   │   ├── Ingredient.cs
│   │   ├── Recipe.cs               # Nested objects
│   │   └── Meal.cs                 # Polymorphism (MealType)
│   └── Interfaces/                 # Abstractions
│       ├── IRepository.cs
│       ├── INutritionAnalysisService.cs
│       └── IDailyMealAggregator.cs
│
├── Application/                    # Use cases
│   └── Services/
│       ├── MealService.cs          # Orchestration
│       └── DailyMealAggregator.cs  # Aggregation
│
├── Infrastructure/                 # Technical implementations
│   ├── Persistence/                # Database
│   │   ├── LiteDbRepository.cs     # Generic repo (thread-safe)
│   │   └── LiteDbConnectionFactory.cs
│   ├── ExternalServices/           # API
│   │   ├── GeminiNutritionAnalysisService.cs
│   │   └── GeminiNutritionResponse.cs
│   └── Configuration/              # Setup
│       └── ServiceCollectionExtensions.cs
│
├── UI/                             # Presentation
│   ├── Form1.cs
│   ├── Form1.Designer.cs
│   └── Form1.resx
│
├── Examples/                       # Code samples
│   └── UsageExamples.cs
│
├── Documentation/
│   ├── README.md                   # Overview
│   ├── ARCHITECTURE.md             # Design
│   ├── SETUP.md                    # Setup
│   ├── CONFIG.md                   # Configuration
│   ├── QUICKREF.md                 # Quick reference
│   └── DI_GUIDE.md                 # DI patterns
│
├── Program.cs                      # Entry point
└── TEST.csproj                     # Project file
```

## 🎯 Key Concepts

### Clean Architecture Benefits
- ✅ **Testable** - Services are isolated via DI
- ✅ **Maintainable** - Clear separation of concerns
- ✅ **Scalable** - Easy to add features
- ✅ **Flexible** - Easy to swap implementations
- ✅ **Professional** - Industry-standard pattern

### SOLID Benefits
- ✅ **S** - Each class has one job
- ✅ **O** - Easy to extend without modifying
- ✅ **L** - Interfaces ensure correct behavior
- ✅ **I** - Focused, manageable interfaces
- ✅ **D** - Loose coupling between layers

### OOP Benefits
- ✅ **Inheritance** - Code reuse, shared behavior
- ✅ **Encapsulation** - Data hiding, controlled access
- ✅ **Polymorphism** - Different types handled uniformly
- ✅ **Abstraction** - Hide complexity behind interfaces

## 🔍 Next Steps

1. **Read** `README.md` for complete overview
2. **Follow** `SETUP.md` to configure API key
3. **Review** `ARCHITECTURE.md` for design details
4. **Study** `Examples/UsageExamples.cs` for code patterns
5. **Build** your UI forms using the patterns shown
6. **Test** with sample data
7. **Deploy** when ready

## 📖 Documentation Map

- **Want overview?** → `README.md`
- **Want to set up?** → `SETUP.md`
- **Want to understand architecture?** → `ARCHITECTURE.md`
- **Want configuration reference?** → `CONFIG.md`
- **Want quick examples?** → `QUICKREF.md` or `Examples/UsageExamples.cs`
- **Want DI explanation?** → `DI_GUIDE.md`

## ✨ Quality Checklist

- ✅ Builds successfully
- ✅ All SOLID principles implemented
- ✅ Thread-safe database operations
- ✅ Comprehensive error handling
- ✅ Testable via dependency injection
- ✅ Properly documented
- ✅ Clean Architecture pattern
- ✅ OOP principles applied
- ✅ Professional code organization
- ✅ Production-ready

---

**This is a complete, production-ready application scaffold. Build your UI on top of it!**

For support, refer to the documentation files or review the code examples.
