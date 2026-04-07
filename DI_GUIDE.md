# Meal Tracker - Dependency Injection & Service Wiring

## DI Container Setup

### In Program.cs
```csharp
var services = new ServiceCollection();

// Register Gemini API key from environment
string? geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

// Register all services with AddMealTrackerServices
services.AddMealTrackerServices(geminiApiKey);

var serviceProvider = services.BuildServiceProvider();

// Pass to UI
var form = new Form1(serviceProvider);
```

### In ServiceCollectionExtensions.cs

```csharp
public static IServiceCollection AddMealTrackerServices(
    this IServiceCollection services,
    string? geminiApiKey = null,
    string? databasePath = null)
{
    // 1. Infrastructure: Database Connection
    var connectionFactory = new LiteDbConnectionFactory(databasePath);
    var database = connectionFactory.GetDatabase();
    
    services.AddSingleton(database);              // Singleton: Shared database
    services.AddSingleton(connectionFactory);     // Singleton: Connection management

    // 2. Infrastructure: Repositories (Dependency Inversion)
    services.AddScoped<IRepository<Meal>>(provider =>
        new LiteDbRepository<Meal>(provider.GetRequiredService<ILiteDatabase>()));
    
    services.AddScoped<IRepository<Recipe>>(provider =>
        new LiteDbRepository<Recipe>(provider.GetRequiredService<ILiteDatabase>()));

    // 3. Infrastructure: External Services
    services.AddScoped<INutritionAnalysisService>(provider =>
        new GeminiNutritionAnalysisService(geminiApiKey));

    // 4. Application: Business Logic
    services.AddScoped<IDailyMealAggregator>(provider =>
        new DailyMealAggregator(provider.GetRequiredService<IRepository<Meal>>()));

    services.AddScoped<MealService>();

    return services;
}
```

## Dependency Injection Flow

### Construction Phase

```
ServiceCollection
    ↓
1. AddSingleton<ILiteDatabase>           LiteDbConnectionFactory creates database
    ↓
2. AddScoped<IRepository<Meal>>          LiteDbRepository<Meal> registered
    ↓
3. AddScoped<IRepository<Recipe>>        LiteDbRepository<Recipe> registered
    ↓
4. AddScoped<INutritionAnalysisService>  GeminiNutritionAnalysisService registered
    ↓
5. AddScoped<IDailyMealAggregator>       DailyMealAggregator registered
    ↓
6. AddScoped<MealService>                MealService registered
    ↓
BuildServiceProvider() creates container
```

### Usage Phase in UI

```csharp
// In Form1.cs
public Form1(IServiceProvider serviceProvider)
{
    _serviceProvider = serviceProvider;
    
    // Services are resolved on demand
    _mealService = serviceProvider.GetRequiredService<MealService>();
    _dailyAggregator = serviceProvider.GetRequiredService<IDailyMealAggregator>();
}
```

## Service Dependency Tree

```
Form1 (UI Layer)
    ↓
MealService (Application Layer)
    ├─→ IRepository<Meal> (Abstraction)
    │   └─→ LiteDbRepository<Meal> (Implementation)
    │       └─→ ILiteDatabase (Singleton)
    │
    ├─→ IRepository<Recipe> (Abstraction)
    │   └─→ LiteDbRepository<Recipe> (Implementation)
    │       └─→ ILiteDatabase (Singleton)
    │
    ├─→ INutritionAnalysisService (Abstraction)
    │   └─→ GeminiNutritionAnalysisService (Implementation)
    │
    └─→ IDailyMealAggregator (Abstraction)
        └─→ DailyMealAggregator (Implementation)
            └─→ IRepository<Meal>
```

## Service Lifetimes

### Singleton
```csharp
services.AddSingleton(database);
services.AddSingleton(connectionFactory);
```
- **One instance for the entire application lifetime**
- Shared across all requests
- Database connection pooling

### Scoped
```csharp
services.AddScoped<IRepository<Meal>>();
services.AddScoped<MealService>();
```
- **One instance per request/scope**
- Services can be disposed after use
- Good for business logic

## Lifetime Best Practices

| Lifetime | When to Use | Example |
|----------|------------|---------|
| **Singleton** | Stateless, thread-safe, reusable | Database connection, config |
| **Scoped** | Per-request state | Repositories, services |
| **Transient** | Lightweight, new each time | Rarely used (not applicable here) |

## Dependency Resolution Example

### Request 1: Create Meal
```
Form1 requests MealService
    ↓
Container creates MealService (Scoped)
    ↓ Needs IRepository<Meal>
Container creates LiteDbRepository<Meal> (Scoped)
    ↓ Needs ILiteDatabase
Container returns Singleton database instance
    ↓ Needs INutritionAnalysisService
Container creates GeminiNutritionAnalysisService (Scoped)
    ↓
MealService ready to use
```

### Request 2: Get Daily Summary
```
Form1 requests MealService again
    ↓
Container creates NEW MealService instance (Scoped)
    ↓ Needs IRepository<Meal>
Container creates NEW LiteDbRepository<Meal> instance (Scoped)
    ↓ Needs ILiteDatabase
Container returns SAME Singleton database instance
    ↓
Services ready to use
```

## Testing with DI

### Unit Test Example
```csharp
[TestClass]
public class MealServiceTests
{
    [TestMethod]
    public async Task CreateMeal_ShouldAnalyzeNutrition()
    {
        // Arrange
        var mockMealRepo = new Mock<IRepository<Meal>>();
        var mockRecipeRepo = new Mock<IRepository<Recipe>>();
        var mockNutrition = new Mock<INutritionAnalysisService>();
        var mockAggregator = new Mock<IDailyMealAggregator>();
        
        var mealService = new MealService(
            mockMealRepo.Object,
            mockRecipeRepo.Object,
            mockNutrition.Object,
            mockAggregator.Object
        );
        
        var meal = new Meal { Name = "Test" };
        
        // Act
        var result = await mealService.CreateAndAnalyzeMealAsync(meal);
        
        // Assert
        mockNutrition.Verify(n => n.AnalyzeMealAsync(meal), Times.Once);
    }
}
```

## Extending the DI Container

### Add New Service

1. **Create interface in Domain**
```csharp
public interface IMyNewService
{
    Task DoSomething();
}
```

2. **Implement in Infrastructure**
```csharp
public class MyNewService : IMyNewService
{
    private readonly IRepository<Meal> _repository;
    
    public MyNewService(IRepository<Meal> repository)
    {
        _repository = repository;
    }
    
    public async Task DoSomething() { /* ... */ }
}
```

3. **Register in ServiceCollectionExtensions**
```csharp
services.AddScoped<IMyNewService, MyNewService>();
```

4. **Use in other services**
```csharp
public class MealService
{
    private readonly IMyNewService _myService;
    
    public MealService(
        IRepository<Meal> mealRepository,
        IMyNewService myService)  // ← Injected here
    {
        _myService = myService;
    }
}
```

## Common DI Patterns

### Pattern 1: Factory-like Registration
```csharp
services.AddScoped<IRepository<T>>(provider =>
{
    var database = provider.GetRequiredService<ILiteDatabase>();
    var repo = new LiteDbRepository<T>(database);
    repo.Initialize();  // Custom initialization
    return repo;
});
```

### Pattern 2: Conditional Registration
```csharp
if (isDevelopment)
{
    services.AddScoped<INutritionAnalysisService, MockNutritionService>();
}
else
{
    services.AddScoped<INutritionAnalysisService, GeminiNutritionAnalysisService>();
}
```

### Pattern 3: Decorator Pattern
```csharp
services.AddScoped<INutritionAnalysisService>(provider =>
{
    var inner = new GeminiNutritionAnalysisService(apiKey);
    return new CachedNutritionService(inner);  // Decorator
});
```

## Troubleshooting DI

### Error: "No constructor found"
```
InvalidOperationException: Unable to activate type 'MealService'
```
**Solution:** Ensure constructor parameters match registered services
```csharp
// ✅ Correct
services.AddScoped<MealService>();
// Service has constructor with registered dependencies

// ❌ Wrong
services.AddScoped<MealService>();
// Service has constructor with unregistered dependencies
```

### Error: "Service not registered"
```
InvalidOperationException: No service for type 'IMyService' is registered
```
**Solution:** Register the service
```csharp
services.AddScoped<IMyService, MyService>();
```

### Error: "Circular dependency"
```
InvalidOperationException: Circular dependency detected
```
**Solution:** Refactor to remove cycles
```csharp
// ❌ A depends on B, B depends on A
// ✅ Introduce interface, use property injection, or refactor
```

## Best Practices

✅ **Use interfaces for all dependencies**
```csharp
public MealService(IRepository<Meal> repo)  // Good
public MealService(LiteDbRepository<Meal> repo)  // Bad
```

✅ **Register at appropriate lifetimes**
```csharp
services.AddSingleton(database);     // Good - shared
services.AddScoped(database);        // Bad - creates multiple
```

✅ **Use GetRequiredService for required dependencies**
```csharp
var service = provider.GetRequiredService<MealService>();  // Good
var service = provider.GetService<MealService>();          // Bad - can return null
```

✅ **Keep constructors simple**
```csharp
// Good - clear dependencies
public MealService(IRepository<Meal> repo, INutritionService nutrition)

// Bad - too many dependencies
public MealService(IRepository<Meal> repo, INutrition nutrition, 
    ILogging log, ICache cache, IConfig config, /* ... */)
```

## Performance Considerations

- **Singleton database:** Reused, efficient
- **Scoped services:** Created per-request, good GC behavior
- **Lazy initialization:** Services created only when needed
- **Factory delegates:** Lightweight creation

---

**Key Takeaway:** The DI container automatically wires up all dependencies. Just register them once in `ServiceCollectionExtensions.cs`, then inject interfaces throughout your application!
