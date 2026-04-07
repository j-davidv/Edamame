# Quick Reference - New UI Features

## 🎯 At a Glance

| Feature | Location | Purpose | How to Access |
|---------|----------|---------|---------------|
| **Dashboard** | Center Panel | See total meals, calories, protein | Click 📊 Dashboard |
| **My Meals** | Center Panel | Browse all meals, view recipes | Click 🍽️ My Meals → "View Recipe" |
| **Daily Log** | Center Panel | Track today's nutrition | Click 📅 Daily Log |
| **Recipe Library** | Center Panel | See macro breakdown for meals | Click 📖 Recipes |
| **Add Meal** | Left Panel | Create new meal | Fill form + Click "Create Meal" |
| **AI Coach** | Right Panel | Get nutrition advice | Type question + Click "Send" |

---

## 🔧 Quick Controls

### Navigation (Left Sidebar)
```
📊 Dashboard  → View total stats
🍽️ My Meals   → Browse meals & recipes
📅 Daily Log  → Today's macro tracking
📖 Recipes    → Macro visualizations
```

### Creating a Meal
```
1. Enter Meal Name
2. Select Meal Type
3. Pick Date
4. List ingredients:
   - 200 grams chicken breast
   - 100 grams brown rice
5. Click "Create Meal"
6. Wait for Gemini analysis
7. Meal automatically saved!
```

### Viewing a Recipe
```
1. Click 🍽️ My Meals
2. Find your meal
3. Click "View Recipe"
4. See all ingredients & nutrition
5. Click back to return
```

### Checking Daily Intake
```
1. Click 📅 Daily Log
2. See today's date
3. View total calories (green box)
4. View total protein (blue box)
5. Adjust meals if needed
```

### Understanding Macros
```
1. Click 📖 Recipes
2. Find your meal
3. See color-coded bars:
   🟢 Green  = Protein %
   🔵 Blue   = Carbs %
   🟠 Orange = Fat %
4. Read exact grams beside each
5. See total calories
```

---

## 🎨 Color Guide

```
🟢 Green  = Protein (Muscle building)
🔵 Blue   = Carbs (Energy)
🟠 Orange = Fat (Essential nutrients)
⬛ Black  = All buttons
```

---

## ⌨️ Keyboard Shortcuts

```
Ctrl + Enter  → Send chat message
Click "View Recipe" → See meal details
Navigation clicks → Switch views
```

---

## 💡 Pro Tips

✅ **Better Nutrition Analysis**
- Be specific: "200g" not "some"
- Use units: "grams", "cups", "tablespoons"
- Check "Recipes" to understand your macros

✅ **Daily Tracking**
- Check "Daily Log" throughout the day
- Watch calories and protein totals
- Create meals to see them count toward daily totals

✅ **Recipe Reminders**
- View recipes before shopping
- See exact ingredients you used
- Recreate favorite meals easily

✅ **AI Advice**
- Ask about nutrition goals
- Request meal suggestions
- Get dietary tips
- Ask "What should I eat tomorrow?"

---

## 📊 Dashboard Stats

What you'll see:
```
Total Meals Recorded: 15
Total Calories: 28,450 kcal
Total Protein: 1,425 g
```

---

## 🍽️ My Meals Layout

```
[Meal Name]     [Type]     [Date]         [Calories]   [Protein]   [View Recipe]
Grilled Chicken Breakfast  Dec 18, 2024   380 kcal     45.2g       [View]
Salmon Bowl     Lunch      Dec 18, 2024   520 kcal     38.5g       [View]
Pasta Dinner    Dinner     Dec 17, 2024   650 kcal     28.3g       [View]
```

---

## 📅 Daily Log Display

```
Daily Summary - Wednesday, December 18, 2024

┌─────────────────┐  ┌──────────────────┐
│   Calories      │  │    Protein       │
│   1,850 kcal    │  │    95.3 g        │
└─────────────────┘  └──────────────────┘
```

---

## 📖 Recipe Library Example

```
🍽️ Grilled Chicken Breast
Protein: 45.2g (60%)    [████████████████░░░░]
Carbs: 15.8g (25%)      [███████░░░░░░░░░░░░]
Fat: 9.5g (15%)         [█████░░░░░░░░░░░░░░]
Total: 385 kcal
```

---

## 🆘 If Something Goes Wrong

| Issue | Solution |
|-------|----------|
| Chat unavailable | Check GOOGLE_API_KEY is set |
| Ingredients not recognized | Use common food names + quantity |
| Meals not showing | Check date filter in Daily Log |
| Numbers look wrong | Verify ingredient format: "200 grams chicken" |

---

## 🚀 Next Steps

1. **Create Your First Meal**
   - Add a meal in the input form
   - Watch Gemini analyze it
   - See it appear in "My Meals"

2. **Explore Your Recipe**
   - Click "View Recipe" on your meal
   - See all ingredients you used
   - Understand the nutrition breakdown

3. **Track Daily**
   - Check "Daily Log" tomorrow
   - Add more meals throughout the day
   - Watch macros accumulate

4. **Understand Your Macros**
   - View "Recipe Library"
   - See macro splits visually
   - Understand your nutrition balance

5. **Get Personal Advice**
   - Ask AI Coach questions
   - Get nutrition tips
   - Plan better meals

---

## 📱 Window Layout Reference

```
Screen (1600 x 700)
┌────────┬───────────┬──────────────────┬─────────┐
│ SIDE   │  INPUT    │  CONTENT         │  CHAT   │
│ NAV    │  FORM     │  (Changes view)  │  PANEL  │
│ 160px  │  360px    │  Flexible width  │  300px  │
└────────┴───────────┴──────────────────┴─────────┘
```

---

## ✨ What's New

- ✅ Complete UI redesign with side navigation
- ✅ Recipe storage and detailed viewing
- ✅ Daily meal tracker with macro totals
- ✅ Visual macro breakdown with colored bars
- ✅ All black buttons with smooth styling
- ✅ Fixed chat panel button placement
- ✅ Dashboard with quick statistics
- ✅ Recipe library with detailed breakdown

---

**Enjoy using the improved Meal Tracker!** 🎉

For detailed information, see:
- `USER_GUIDE_NEW_FEATURES.md` - Complete feature guide
- `UI_REDESIGN_SUMMARY.md` - Technical details
- `IMPLEMENTATION_COMPLETE.md` - Full implementation notes
