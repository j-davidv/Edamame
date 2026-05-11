# Edamame



## Project Title
Edamame — Meal planner and nutrition analysis desktop app

## Project Description and Purpose
Edamame is a WinForms desktop application for composing meals from recipes, parsing ingredient lines, and producing nutrition estimates and simple dietary advice. The primary purpose is to let users build meals, calculate calories and macronutrients, and get easy-to-understand guidance based on analyzed nutrition data.

## UML Diagram

<img src="Assets/Meal%20Analysis%20Framework-2026-05-11-121649.png" alt="UML Class Diagram - Edamame Architecture" width="900" />

[View full-size diagram](Assets/Meal%20Analysis%20Framework-2026-05-11-121649.png)

## Features and Functionalities
- Create, edit and combine recipes into meals
- Parse ingredient lines and normalize quantity/unit/name
- Analyze meals and recipes for calories, protein, carbs, fat, sodium, sugar, saturated fat
- Provide basic dietary classification and short dietary advice
- In-memory caching for nutrition lookups to reduce repeated API calls
- WinForms UI for creating meals, viewing analysis, and editing recipes

## How the program works
1. User creates or edits recipes with ingredient lines.
2. Ingredients are parsed and normalized by `IngredientParser` (quantity, unit, name).
3. When a meal is analyzed the app collects all ingredient lines and calls an `INutritionAnalysisService` implementation (e.g., `GeminiNutritionAnalysisService`) which sends a deterministic prompt to the AI/third-party nutrition service and expects a JSON response with nutrient fields.
4. The response is parsed into `NutritionalMetric` and cached in memory keyed by normalized ingredient set.
5. The UI displays summarized nutrition metrics and a short dietary classification/advice string.
6. Repositories (LiteDB) persist recipes and meals locally.

## Instructions to run the application
Prerequisites:
- .NET 10 SDK installed: https://dotnet.microsoft.com
- Visual Studio Community 2026 (recommended) or VS Code

Run in Visual Studio:
1. Open `Edamame.sln` in Visual Studio
2. Restore NuGet packages
3. Build the solution (target .NET 10)
4. Set the WinForms project in `Presentation/` as the Startup Project and run (F5)

Run from CLI:
```powershell
dotnet build
dotnet run --project Presentation/Presentation.csproj
```

Configuration:
- Provide any required API keys (example for Gemini-based analysis):
  - Windows PowerShell: `$env:GEMINI_API_KEY = "your_gemini_api_key_here"`
  - Or set in your OS environment variables / secrets store used by your configuration.

Notes:
- The AI-based nutrition service expects a deterministic output (temperature=0) and returns a JSON object which the service parses; malformed responses may cause an error.

## Developers / Team Members
- John David M. Villan
- Ivan C. Desierto
- Russell Matthew Tañedo