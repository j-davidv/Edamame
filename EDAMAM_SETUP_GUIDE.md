# EDAMAM Integration & Modern UI - Complete Setup Guide

## Overview
The Meal Tracker has been completely refactored:
- ✅ **Removed**: All Gemini API integration
- ✅ **Added**: EDAMAM Nutrition Analysis API (Free Plan)
- ✅ **Redesigned**: Modern, clean Windows Forms UI
- ✅ **Streamlined**: Simplified meal creation workflow

## EDAMAM API Setup

### 1. Get Your Credentials
1. Go to https://developer.edamam.com/
2. Sign up for a free account
3. Go to **Applications** → **Create new application**
4. Select **Nutrition Analysis API**
5. Fill in the form and create the app
6. You'll receive:
   - **Application ID** (App ID)
   - **Application Keys** (App Key)

### 2. Set Environment Variables

#### Windows PowerShell:
```powershell
# Set temporarily (for current session)
$env:EDAMAM_APP_ID = "your_app_id_here"
$env:EDAMAM_APP_KEY = "your_app_key_here"

# Verify
Write-Output $env:EDAMAM_APP_ID
Write-Output $env:EDAMAM_APP_KEY
```

#### Windows Command Prompt:
```cmd
set EDAMAM_APP_ID=your_app_id_here
set EDAMAM_APP_KEY=your_app_key_here
```

#### Permanent (Windows Environment Variables):
1. Right-click **This PC** → **Properties**
2. Click **Advanced system settings**
3. Click **Environment Variables**
4. Under **User variables**, click **New**
5. Variable name: `EDAMAM_APP_ID` → Value: `your_app_id`
6. Click **New** again
7. Variable name: `EDAMAM_APP_KEY` → Value: `your_app_key`
8. Click **OK** and **OK** again
9. **Restart your application**

## Project Structure Changes

### Removed Files
- `Infrastructure/ExternalServices/GeminiNutritionAnalysisService.cs`
- `Infrastructure/ExternalServices/GeminiNutritionResponse.cs`

### New Files
- `Infrastructure/ExternalServices/EdamamNutritionAnalysisService.cs` - EDAMAM API client

### Updated Files
- `Program.cs` - Now reads EDAMAM credentials
- `Infrastructure/Configuration/ServiceCollectionExtensions.cs` - EDAMAM registration
- `Form1.Designer.cs` - Modern UI layout
- `Form1.cs` - New event handlers for modern UI

##Modern UI Features

### Layout
- **Left Panel**: Input form for creating meals
  - Meal name input
  - Meal type selector (Breakfast, Brunch, Lunch, Snack, Dinner, Supper)
  - Date picker
  - Recipes/Ingredients text box
  - Create and Analyze buttons

- **Right Panel**: Meal data grid
  - Displays all meals with nutrition data
  - Sortable columns: Name, Type, Date, Calories, Protein, Carbs, Fat
  - Delete, Refresh, and Daily Summary buttons

### Color Scheme
- Primary Green: Create buttons (#4CAF50)
- Primary Blue: Analyze buttons (#2196F3)
- Error Red: Delete buttons (#F44336)
- Neutral Gray: Refresh buttons (#9E9E9E)
- Warning Orange: Summary buttons (#FF9800)

### Design Philosophy
- Clean, minimal interface
- Proper spacing and padding
- Modern flat design (no gradients)
- Accessible font sizes
- Clear visual hierarchy

## How to Use

### 1. Create a Meal
1. Enter **Meal Name** (e.g., "Grilled Chicken Salad")
2. Select **Meal Type** from dropdown
3. Set **Date** using date picker
4. Enter **Recipes & Ingredients** (format: `<quantity> <unit> <ingredient>`)
   - Example:
     ```
     200 gram chicken breast
     100 gram lettuce
     50 gram olive oil
     10 gram salt
     ```
5. Click **Create Meal**
6. Nutritional data will be automatically calculated via EDAMAM API

### 2. View Meal Details
- Select a meal from the grid
- Click **Analyze** to see detailed nutritional breakdown
- Shows: Calories, Protein, Carbs, Fat

### 3. Delete a Meal
- Select a meal from the grid
- Click **Delete Selected**
- Confirm deletion

### 4. View Daily Summary
- Select a date using date picker
- Click **Daily Summary**
- Shows aggregate nutritional data for that day

## Ingredient Format

The ingredient parser expects: `<quantity> <unit> <ingredient>`

### Examples:
```
200 gram chicken breast
250 ml milk
1 cup rice
50 g olive oil
2 tablespoon honey
500 ml water
100 gram tomato
```

## Nutritional Data

The EDAMAM API calculates:
- **Calories** (kcal)
- **Protein** (grams)
- **Carbohydrates** (grams)
- **Fat** (grams)
- **Sodium** (mg)
- **Sugar** (grams)
- **Saturated Fat** (grams)

## EDAMAM API Limits (Free Plan)

- **Requests per minute**: 60
- **Requests per day**: 500
- **Maintained by**: Edamam
- **Documentation**: https://developer.edamam.com/

## Error Handling

### Common Issues

| Error | Solution |
|-------|----------|
| "EDAMAM API credentials are required" | Set EDAMAM_APP_ID and EDAMAM_APP_KEY environment variables |
| "HTTP error calling EDAMAM API" | Check API credentials; verify internet connection |
| "Meal has no ingredients to analyze" | Make sure to enter at least one ingredient |
| "No text content in response" | EDAMAM API may be rate-limited; try again later |

## Status Bar Messages

- `Ready` - Application is idle
- `Loaded X meals` - Successfully loaded meals
- `Analyzing meal nutrition with EDAMAM...` - API call in progress
- `✓ Meal 'X' created with Y calories` - Meal created successfully
- `Meal 'X' deleted` - Meal deleted successfully
- `Daily summary calculated` - Summary display ready

## Database

- **Type**: LiteDB (file-based NoSQL)
- **Location**: `%APPDATA%\MealTracker\meals.db`
- **Auto-created**: On first meal creation
- **Thread-safe**: Yes (ReaderWriterLockSlim)

## Building & Running

### Build
```bash
dotnet build
```

### Run
```bash
# Make sure environment variables are set first
dotnet run
```

### Visual Studio
- Press `F5` to debug
- Press `Ctrl+F5` to run
- Set environment variables before launching

## Architecture

### Layers
1. **Presentation** (Form1.cs/Designer.cs) - Windows Forms UI
2. **Application** (MealService) - Business logic
3. **Domain** (Entities, Interfaces) - Core models
4. **Infrastructure** (Repositories, API Clients, Persistence) - Data access

### Dependencies
- `Google.GenAI`: Removed (was Gemini)
- `LiteDB`: Database persistence
- `Microsoft.Extensions.DependencyInjection`: Dependency injection
- `System.Net.Http.Json`: HTTP client for EDAMAM

## Comparison: Gemini → EDAMAM

| Aspect | Gemini | EDAMAM |
|--------|--------|---------|
| API Model | LLM (AI) | Nutrition Database |
| Speed | Slower (AI processing) | Faster (direct lookup) |
| Accuracy | Variable (AI-generated) | High (curated database) |
| Cost | Free, but limited | Free plan available |
| Setup | Complex | Simple (ID + Key) |
| Response Format | JSON (flexible) | JSON (structured) |
| Error Rate | Higher | Lower |
| Use Case | General nutrition | Accurate nutritional data |

## Notes

- Meals are stored with precise EDAMAM nutritional data
- Daily summaries aggregate all meals for a given date
- The UI is responsive and handles async operations smoothly
- All controls are properly disposed on form close

## Support

For issues with:
- **EDAMAM API**: Visit https://developer.edamam.com/docs
- **LiteDB**: Visit https://www.litedb.org/
- **This Application**: Check error messages in status bar

## Next Steps

1. ✅ Set EDAMAM environment variables
2. ✅ Build the project
3. ✅ Run the application
4. ✅ Create your first meal
5. ✅ View nutritional data
6. ✅ Enjoy meal tracking!
