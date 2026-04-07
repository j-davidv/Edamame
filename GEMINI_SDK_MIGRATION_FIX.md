# Gemini API SDK Migration - JSON Parsing Fix

## Problem
When creating a meal, the application was throwing a `JsonException` with the message:
```
Failed to parse Gemini API response as JSON
```

**Root Causes:**
1. The official `Google.GenAI` SDK might return JSON wrapped in markdown code blocks (```json ... ```)
2. The simple string-based API call wasn't being properly configured
3. Missing explicit instruction to return plain JSON without formatting

## Solution Implemented

### 1. Updated `GeminiNutritionAnalysisService.cs`

**Key Changes:**
- Added markdown code block cleanup logic in `CallGeminiApiAsync`
- Enhanced prompt to explicitly request JSON-only responses without markdown
- Added better error handling with more descriptive messages

```csharp
private async Task<string> CallGeminiApiAsync(string prompt)
{
    try
    {
        // Add explicit instruction for JSON-only response
        var jsonPrompt = $"""
        {prompt}
        
        IMPORTANT: Respond with ONLY a valid JSON object, no additional text or markdown formatting.
        Do NOT wrap the JSON in code blocks or any other text.
        """;

        var response = await _client.Models.GenerateContentAsync(
            model: ModelName,
            contents: jsonPrompt
        );

        var textContent = response?.Candidates?[0]?.Content?.Parts?[0]?.Text;

        if (string.IsNullOrWhiteSpace(textContent))
        {
            throw new InvalidOperationException("No text content in Gemini API response...");
        }

        // Clean up markdown code blocks if present
        var cleanedText = textContent.Trim();
        if (cleanedText.StartsWith("```json"))
            cleanedText = cleanedText.Substring(7); // Remove ```json
        else if (cleanedText.StartsWith("```"))
            cleanedText = cleanedText.Substring(3); // Remove ```

        if (cleanedText.EndsWith("```"))
            cleanedText = cleanedText.Substring(0, cleanedText.Length - 3); // Remove closing ```

        return cleanedText.Trim();
    }
    // ... error handling
}
```

### 2. Updated NuGet Package Reference

**File:** `TEST.csproj`
```xml
<PackageReference Include="Google.GenAI" Version="1.0.0" />
```

### 3. Error Handling Improvements

Enhanced exception messages to help with debugging:
- `AnalyzeMealAsync`: "Error analyzing meal: {ex.Message}"
- `AnalyzeRecipeAsync`: "Error analyzing recipe: {ex.Message}"
- JSON parsing: "Failed to parse Gemini API response as JSON. Ensure the response is valid JSON."

## Testing Instructions

1. **Rebuild the application:**
   ```
   dotnet build
   ```

2. **Ensure API key is set:**
   ```powershell
   $env:GEMINI_API_KEY = "your_actual_api_key"
   ```

3. **Run the application:**
   ```
   dotnet run
   ```

4. **Test meal creation:**
   - Enter meal name: "Grilled Chicken with Rice"
   - Select type: "Lunch"
   - Click "Create Meal"
   - Verify nutritional data displays in grid

## Expected Behavior After Fix

✅ Meal creation succeeds without JSON parsing errors
✅ Nutritional metrics display correctly: Calories, Protein, Carbs, Fat, Sodium, Sugar, Saturated Fat
✅ Dietary classification appears (e.g., "High Protein", "Balanced")
✅ Dietary advice displays
✅ Grid refreshes with new meal data

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Still getting JSON parse error | Verify Gemini API key is valid |
| "No text content" error | Check internet connection; Gemini API may be unreachable |
| Timeout errors | API is slow; retry the operation |
| Invalid nutritional values | Model may be returning unexpected format; check API response |

## Architecture

The fixed service now:
1. Uses official `Google.GenAI` SDK (more reliable)
2. Handles markdown formatting in responses (common with GenAI models)
3. Provides clear error messages for debugging
4. Cleanly extracts JSON from potentially wrapped responses

## Migration Summary

| Aspect | Before | After |
|--------|--------|-------|
| HTTP Client | Manual HttpClient with raw requests | Official Google.GenAI SDK |
| API Endpoint | Manual URL construction | SDK handles endpoint |
| Response Handling | Direct parsing | Markdown cleanup + parsing |
| Error Messages | Generic | Specific and diagnostic |
| JSON Schema | Manual schema in prompt | Schema passed to API |
