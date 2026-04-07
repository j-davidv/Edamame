# Meal Tracker - Quick Start Guide

## Starting the Application

```bash
dotnet run
```

The Meal Tracker window will open with a professional Windows Forms interface ready to use.

## Core Workflow

### Creating Your First Meal

1. **Enter Meal Details**:
   - Type meal name (e.g., "Grilled Chicken Salad")
   - Select meal type (Breakfast, Lunch, Dinner, etc.)
   - Optionally add notes (e.g., "Healthy lunch option")
   - Date defaults to today (change if needed)

2. **Create the Meal**:
   - Click "Create Meal" button
   - Application will:
     - Call Gemini API to analyze nutritional content
     - Save meal to database
     - Refresh the meals grid
   - You'll see a success message

3. **View Your Meal**:
   - Meal appears in the grid on the "Meals" tab
   - Shows: Name, Type, Date, Calories, Protein, Carbs, Fat
   - All data persisted in local LiteDB database

### Analyzing a Meal

1. **Select a meal** from the grid (click the row)
2. **Click "Analyze" button**
3. **Detailed window appears** showing:
   - Complete nutritional breakdown
   - Dietary classification
   - Personalized health recommendations

### Viewing Daily Summary

1. **Switch to "Daily Summary" tab**
2. **Select a date** (or leave as today)
3. **Click "Show Summary" button**
4. **View formatted summary** with:
   - Total meals for the day
   - Aggregated nutritional totals
   - Daily dietary classification
   - Health recommendations

### Managing Meals

**To Delete**:
1. Select a meal from grid
2. Click "Delete Selected"
3. Confirm in dialog
4. Meal removed from database

**To Refresh**:
- Click "Refresh" button anytime
- Grid reloads with latest data

## Important Requirements

### Environment Variable
Before first run, set your Gemini API key:

**PowerShell**:
```powershell
$env:GEMINI_API_KEY = "your_api_key_here"
```

**Command Prompt**:
```cmd
set GEMINI_API_KEY=your_api_key_here
```

**Permanent (Windows)**:
1. Windows + X → System
2. Advanced system settings
3. Environment Variables
4. Add `GEMINI_API_KEY` with your key value
5. Restart the app

### Get Your Free API Key
1. Visit https://ai.google.dev
2. Click "Get API Key"
3. Create or select a Google Cloud project
4. Copy your key (starts with "AIza...")

## Database

- **Location**: `%APPDATA%\MealTracker\meals.db`
- **Type**: LiteDB (file-based, no server needed)
- **Auto-created**: First meal you create generates database
- **Thread-safe**: Multiple operations can run concurrently
- **Backup**: Copy meals.db to backup meals

## UI Components Explained

### Meals Tab (Default View)

```
┌─────────────────────────────────────────────┐
│ Meal Name: [_____________] Type: [Breakfast]│  ← Input section
│ Date: [01/15/2024] Notes: [_____________]   │
│ [Create Meal] [Analyze]                     │
├─────────────────────────────────────────────┤
│ Today's Meals:                              │
│ ┌───────────────────────────────────────┐   │
│ │ Name │ Type │ Date │ Cal │ Protein │ │   │  ← Results grid
│ │ Oats │ Brkf │ 1/15 │ 350 │ 12.5 │   │
│ └───────────────────────────────────────┘   │
│ [Delete Selected] [Refresh]                 │
└─────────────────────────────────────────────┘
```

### Daily Summary Tab

```
┌─────────────────────────────────────────────┐
│ Date: [01/15/2024] [Show Summary]           │  ← Date selector
├─────────────────────────────────────────────┤
│ 📅 Date: 01/15/2024                         │
│ 🍽️ Meals: 3                                │
│                                             │
│ 📊 Daily Totals:                            │  ← Summary display
│   🔥 Calories: 2150 kcal                    │
│   💪 Protein: 95.5g                         │
│   ...                                       │
└─────────────────────────────────────────────┘
```

## Common Tasks

### Find Meals from a Specific Date
1. Go to Meals tab
2. Change date picker to desired date
3. Click Refresh
4. Grid shows meals for that date

### Compare Nutritional Content
1. Create multiple meals on same date
2. Go to Daily Summary tab
3. Select that date and Show Summary
4. See total nutritional intake for the day

### Keep History
- All meals automatically saved
- Change date picker to view any previous day
- Delete only removes from database (no undo)

### Improve Meal Analysis
- Add detailed notes to meal
- Include ingredients in notes for better AI analysis
- Gemini API uses meal name and notes for analysis

## Tips & Tricks

1. **Batch Create**: Create multiple meals quickly
   - Enter first meal, click Create
   - Grid refreshes automatically
   - Continue with next meal without refreshing

2. **Accurate Measurements**: 
   - Be specific in meal name (e.g., "200g Grilled Chicken")
   - Add ingredient details in notes
   - Results in more accurate analysis

3. **Date Navigation**:
   - Meal date picker shows today by default
   - Use for meals on different dates
   - Summary date picker independent for viewing

4. **Quick Delete**:
   - Select meal and press Delete button
   - Confirm immediately if certain
   - No way to undo from UI (but can restore from database backup)

5. **Performance**:
   - First meal creation takes longer (API call)
   - Subsequent meals faster
   - Summary generation aggregates all meals for date

## Error Messages

| Error | Solution |
|-------|----------|
| "API key is required" | Set GEMINI_API_KEY environment variable |
| "Failed to create meal" | Check API key validity and network |
| "Access to path denied" | Ensure write permissions in AppData folder |
| "JSON deserialization failed" | Check API key is still valid |

## Keyboard Navigation

| Key | Action |
|-----|--------|
| Tab | Move between controls |
| Enter | Click focused button |
| Alt+M | Switch to Meals tab |
| Alt+D | Switch to Daily Summary tab |

## Next Steps

1. **Create your first meal** to test the system
2. **View the daily summary** to see aggregated data
3. **Experiment with dates** to organize meals
4. **Check database** at `%APPDATA%\MealTracker\meals.db`
5. **Explore API response** in detailed meal analysis

## Support & Troubleshooting

**Meals not appearing?**
- Ensure meal was created (check for success message)
- Verify date picker shows correct date
- Click Refresh to reload grid

**API errors during meal creation?**
- Check internet connection
- Verify GEMINI_API_KEY is set correctly
- Check free tier rate limits (60 RPM)

**Database issues?**
- Don't modify or move meals.db while app is running
- Close app before backup/restore
- Database auto-creates on first meal

**UI not responding?**
- Status bar shows current operation
- Long API calls show "Creating and analyzing meal..."
- Wait for completion before next action

## Professional Features

✅ **Thread-Safe Database**: Multiple operations without conflicts  
✅ **Async/Await**: Responsive UI during long operations  
✅ **Error Handling**: User-friendly messages for all errors  
✅ **Data Validation**: Required fields enforced  
✅ **Status Updates**: Real-time feedback on operations  
✅ **Date Navigation**: View any date's meals and summary  
✅ **Persistent Storage**: All data saved to database  
✅ **API Integration**: Automatic nutritional analysis  

## License

This application is provided for educational and commercial use with proper attribution.
