# Meal Tracker Configuration Guide

## Environment Variables

Create a `.env` file in the project root (optional, for local development):

```
GEMINI_API_KEY=AIza...your_api_key_here...
DATABASE_PATH=C:\MealTrackerData\meals.db
```

### Method 1: Environment Variable (Recommended)

**Windows Command Prompt:**
```cmd
setx GEMINI_API_KEY "AIza...your_key..."
```

**Windows PowerShell:**
```powershell
[Environment]::SetEnvironmentVariable("GEMINI_API_KEY", "AIza...your_key...", "User")
```

**Linux/macOS:**
```bash
export GEMINI_API_KEY="AIza...your_key..."
```

### Method 2: Hard-coded in Program.cs (Development Only)

In `Program.cs`, replace:
```csharp
string? geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
```

With:
```csharp
string? geminiApiKey = "AIza...your_actual_api_key...";
```

⚠️ **Never commit API keys to version control!**

### Method 3: Configuration File (Production)

Create `appsettings.json`:
```json
{
  "GeminiSettings": {
    "ApiKey": "AIza...your_key...",
    "Model": "gemini-1.5-flash"
  },
  "DatabaseSettings": {
    "Path": "C:\\MealTrackerData\\meals.db"
  }
}
```

Update `ServiceCollectionExtensions.cs`:
```csharp
public static IServiceCollection AddMealTrackerServices(
    this IServiceCollection services,
    IConfiguration configuration)
{
    var apiKey = configuration["GeminiSettings:ApiKey"];
    var dbPath = configuration["DatabaseSettings:Path"];
    
    return services.AddMealTrackerServices(apiKey, dbPath);
}
```

## Database Configuration

### Default Location
```
%APPDATA%\MealTracker\meals.db
```

### Custom Location

In `Program.cs`:
```csharp
services.AddMealTrackerServices(
    geminiApiKey: apiKey,
    databasePath: "C:\\CustomPath\\meals.db"
);
```

### Backup Database

The database file can be backed up like any regular file:
```bash
xcopy "%APPDATA%\MealTracker" "D:\Backup\MealTracker" /Y /I
```

## API Rate Limits

**Gemini Free Tier:**
- Requests: Up to 60 RPM (Requests Per Minute)
- Daily: Up to 1,500 requests per day
- Payload: Max 4,096 tokens per request

**Optimization:**
- Batch meals for analysis
- Cache results when possible
- Implement retry logic with exponential backoff

## Security Best Practices

1. **Never log API keys**
   ```csharp
   // ❌ Don't do this
   Console.WriteLine($"Using API key: {apiKey}");
   
   // ✅ Do this
   Console.WriteLine("API key configured successfully");
   ```

2. **Use environment variables**
   - Never hardcode secrets
   - Use Azure Key Vault for production
   - Rotate keys regularly

3. **Encrypt sensitive data**
   - Consider encrypting the LiteDB file for production
   - Use Data Protection API (DPAPI) on Windows

4. **Input validation**
   - All user inputs are validated before processing
   - Malformed JSON responses are caught and logged

## Proxy Configuration (Corporate Networks)

If behind a corporate proxy:

```csharp
// In GeminiNutritionAnalysisService constructor
var handler = new HttpClientHandler
{
    Proxy = new WebProxy("http://proxy.company.com:8080"),
    UseProxy = true
};
_httpClient = new HttpClient(handler);
```

## Logging Configuration

Add Serilog for production logging:

```bash
dotnet add package Serilog
dotnet add package Serilog.Sinks.File
```

```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    // Application code
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

## Performance Tuning

### Database Connection Pool
LiteDB uses shared connections by default. For high concurrency:

```csharp
_connectionString = $"Filename={dbFile};Upgrade=true;Cache=true;";
```

### API Request Timeout
In `GeminiNutritionAnalysisService`:

```csharp
_httpClient.Timeout = TimeSpan.FromSeconds(30);
```

### Cache Frequently Requested Data
In `DailyMealAggregator`:

```csharp
private static readonly Dictionary<DateTime, NutritionalMetric> _cache = new();

public async Task<NutritionalMetric> GetDailyTotalsAsync(DateTime date)
{
    if (_cache.TryGetValue(date.Date, out var cached))
        return cached;
    
    var totals = await CalculateTotalsAsync(date);
    _cache[date.Date] = totals;
    return totals;
}
```

## Deployment Checklist

- [ ] API key set in environment variables
- [ ] Database path accessible and writable
- [ ] .NET 10 runtime installed on target machine
- [ ] LiteDB version 5.0.17 available
- [ ] Network proxy configured (if needed)
- [ ] Application tested with production API key
- [ ] Backup strategy in place
- [ ] Logging configured
- [ ] Error handling tested
- [ ] Performance requirements met

## Troubleshooting

### "API key is required" Error
```csharp
// Check if environment variable is set
var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
Console.WriteLine(apiKey == null ? "NOT SET" : "SET");
```

### "Access to the path denied"
```powershell
# Check folder permissions
icacls "%APPDATA%\MealTracker"

# Grant write permissions
icacls "%APPDATA%\MealTracker" /grant "%USERNAME%":F /T
```

### "Connection timeout"
```csharp
// Increase timeout in API service
_httpClient.Timeout = TimeSpan.FromSeconds(60);
```

### "Port already in use"
LiteDB doesn't use ports; check if another instance is running:
```powershell
# Check running processes
Get-Process | Where-Object {$_.ProcessName -like "*meal*" -or $_.ProcessName -like "*test*"}
```

## Advanced Configuration

### Custom JSON Schema for API Responses
Edit `GeminiNutritionAnalysisService.cs`:

```csharp
private const string JsonSchema = """
{
    "type": "object",
    "properties": {
        "nutritional_metrics": { /* ... */ },
        "custom_field": { "type": "string" }
    }
}
""";
```

### Custom Unit System
In `Ingredient.cs`:
```csharp
public enum UnitSystem
{
    Metric,     // grams, ml
    Imperial,   // ounces, cups
    Custom      // any string
}
```

### Multi-Language Support
Create localization files:
```
Localization/
├── en-US.json
├── es-ES.json
└── fr-FR.json
```

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review `ARCHITECTURE.md` for design details
3. Check `Examples/UsageExamples.cs` for code patterns
4. Review error messages in console/logs carefully

---

**Last Updated:** 2024
**Version:** 1.0
