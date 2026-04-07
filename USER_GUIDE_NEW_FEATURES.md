# New Features Guide - Meal Tracker Application

## Getting Started with the Redesigned UI

### Main Layout Overview

When you launch the application, you'll see 4 main sections:

1. **Left Panel (Dark)**: Navigation menu
2. **Left-Middle Panel (White)**: Add Meal form
3. **Center Panel (Large)**: Dynamic content area
4. **Right Panel (White)**: AI Nutrition Coach chat

---

## Features Guide

### 📊 Dashboard
**Purpose**: Quick overview of your meal tracking progress

**What you'll see:**
- Total number of meals recorded
- Total calories across all meals
- Total protein consumed across all meals

**How to use:**
1. Click the "📊 Dashboard" button in the left navigation
2. View your meal statistics at a glance
3. Use this to track overall progress

---

### 🍽️ My Meals
**Purpose**: Browse, search, and view recipes of all your meals

**What you'll see:**
- List of all meals you've created
- Meal name, type, date, and nutrition data
- A "View Recipe" button for each meal

**How to use:**
1. Click the "🍽️ My Meals" button in the left navigation
2. (Optional) Use the search box to find specific meals
3. Click "View Recipe" on any meal to see:
   - Meal type and date
   - Complete list of ingredients with quantities
   - Full nutritional breakdown (calories, protein, carbs, fat)
4. Use this to remind yourself how you made a particular meal

---

### 📅 Daily Log
**Purpose**: Track calories and protein for the current day

**What you'll see:**
- Current date and day of week
- Total calories consumed today (green box)
- Total protein consumed today (blue box)

**How to use:**
1. Click the "📅 Daily Log" button in the left navigation
2. View today's macro statistics
3. Meals created today are automatically included
4. Check back throughout the day to track your intake

**Example:**
```
Daily Summary - Wednesday, December 18, 2024
┌──────────────┐  ┌──────────────┐
│  Calories    │  │  Protein     │
│  1,850 kcal  │  │  95.3 g      │
└──────────────┘  └──────────────┘
```

---

### 📖 Recipes (Recipe Library)
**Purpose**: View all your recipes with macro nutrient breakdowns

**What you'll see:**
- Each recipe displayed with a macro breakdown
- Color-coded macro bars:
  - 🟢 **Green** = Protein (%)
  - 🔵 **Blue** = Carbohydrates (%)
  - 🟠 **Orange** = Fat (%)
- Total calorie count for each recipe

**How to use:**
1. Click the "📖 Recipes" button in the left navigation
2. Scroll through your recipes
3. Each recipe shows:
   - Visual bars showing macro split
   - Exact grams of protein, carbs, and fat
   - Percentage of total macros
   - Total calories

**Example:**
```
🍽️ Grilled Chicken Breast
├─ Protein: 45.2g (60%)  [████████████████░░░░]
├─ Carbs: 15.8g (25%)    [███████░░░░░░░░░░░░]
├─ Fat: 9.5g (15%)       [█████░░░░░░░░░░░░░░]
└─ Total: 385 kcal
```

---

## Creating New Meals

### How to Add a Meal

1. **Fill in the Add Meal form** (left panel):
   - **Meal Name**: Give your meal a name (e.g., "Grilled Chicken & Vegetables")
   - **Meal Type**: Select from Breakfast, Brunch, Lunch, Snack, Dinner, or Supper
   - **Date**: Pick the date for this meal (defaults to today)
   - **Recipes & Ingredients**: Enter each ingredient on a new line

2. **Format for ingredients**:
   ```
   200 grams chicken breast
   100 grams brown rice
   50 grams olive oil
   ```
   
   Or simply:
   ```
   200 g chicken breast
   1 serving brown rice
   1 tablespoon olive oil
   ```

3. **Click "Create Meal"**
   - The app sends your ingredients to Gemini AI
   - AI calculates nutrition (calories, protein, carbs, fat)
   - Your meal is saved to the database
   - You can now view it in "My Meals"

---

## Using the AI Nutrition Coach

**Purpose**: Get personalized nutrition advice and ask dietary questions

**Located**: Right panel of the application

### How to Use:
1. Click in the **message box** at the bottom of the chat panel
2. Type your question or request, for example:
   - "Is my protein intake good for muscle building?"
   - "What foods are high in fiber?"
   - "How many calories should I aim for?"
   - "Suggest a healthy breakfast"
3. Click "Send" or press **Ctrl+Enter**
4. The AI coach will respond with personalized advice

### Chat Features:
- **Clear Button**: Click to clear chat history
- **Conversation Memory**: The coach remembers your conversation context
- Works best with nutrition and meal-related questions

---

## Button Color & Styling

All buttons in the application now feature:
- **Black background** (#000000)
- **Smooth, flat design** (no 3D effects)
- **Hover effect**: Darker shade when you mouse over
- **Consistent styling** across all sections

---

## Tips & Tricks

✅ **Best Practices:**
- Use consistent ingredient names (e.g., always "chicken breast" not "breast chicken")
- Include quantity and unit for accurate nutritional analysis
- Record meals in the "My Meals" section to build your recipe library
- Check "Daily Log" throughout the day to stay on track
- Use the "Recipes" section to understand your macro ratios
- Ask the AI Coach for dietary advice - it's trained in nutrition!

🎯 **Workflow Suggestion:**
1. Create a meal → it auto-analyzes nutrition
2. View it in "My Meals" → understand ingredients
3. Check "Daily Log" → track daily intake
4. View "Recipes" → understand macro breakdown
5. Ask AI Coach → get personalized advice

❓ **Common Questions:**

**Q: Can I edit a meal after creating it?**
- Currently, you can view and delete meals
- To modify, delete and recreate with new ingredients

**Q: What if my ingredients aren't recognized?**
- Use common food names (e.g., "chicken breast" not "poultry")
- Be specific with units (grams, cups, tablespoons)
- The AI Coach can help identify good alternatives

**Q: How accurate is the nutrition data?**
- Based on USDA database via Gemini AI
- Generally accurate within 5-10% for common foods
- Serve accurate nutrition for homemade meals vs restaurant food

**Q: Can I track multiple days?**
- Yes! Create meals with different dates
- Daily Log shows only today's totals
- Dashboard shows all-time totals

---

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| **Ctrl+Enter** | Send chat message |
| **Click "View Recipe"** | Open meal recipe details |
| **Navigation Buttons** | Switch between Dashboard, Meals, Log, and Recipes |

---

## Troubleshooting

### Issue: Chat says "Gemini Chat Service not available"
- **Solution**: Ensure your `GOOGLE_API_KEY` environment variable is set
- Check `appsettings.json` for the API key
- Restart the application

### Issue: Ingredients not being recognized
- **Solution**: Use standard food names and include quantity with units
- Example: "200 grams salmon" instead of "some salmon"

### Issue: Meals not appearing in My Meals
- **Solution**: Refresh the meal list or check the date filter
- Make sure the meal was created for today's date in the Daily Log

---

## Application Hotkeys Reference

- **Left Navigation**: Click any button to switch views
- **Add Meal Form**: Fill out and click "Create Meal"
- **Chat Panel**: Type message, press Ctrl+Enter or click Send
- **Recipe Viewing**: Click "View Recipe" in My Meals list

---

## What's New in This Update

✅ **Redesigned UI** with side navigation
✅ **Recipe Storage** - view all ingredients for past meals
✅ **Daily Meal Log** - MyFitnessPal style tracking
✅ **Macro Breakdown** - visual representation of nutrition splits
✅ **Dashboard** - quick stats overview
✅ **Black Button Styling** - modern, clean design
✅ **Recipe Library** - all meals with macro visualization
✅ **Improved Layout** - 4-column modern design

---

For more technical details, see `UI_REDESIGN_SUMMARY.md`.
