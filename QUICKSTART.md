# Quick Start - EDAMAM Meal Tracker

## ⚡ Quick Setup (5 minutes)

### 1. Get EDAMAM Credentials
```
1. Visit: https://developer.edamam.com/
2. Sign up → Create Account
3. Create Application → Nutrition Analysis API
4. Copy: App ID and App Key
```

### 2. Set Environment Variables
```powershell
$env:EDAMAM_APP_ID = "paste_your_app_id"
$env:EDAMAM_APP_KEY = "paste_your_app_key"
```

### 3. Run the App
```bash
dotnet build
dotnet run
```

---

## 🎯 Using the Meal Tracker

### Create a Meal (2 steps)

**Step 1: Fill in the form**
```
Meal Name: Grilled Chicken Salad
Meal Type: Lunch
Date: Today
Recipes & Ingredients:
  200 gram chicken breast
  100 gram lettuce  
  50 ml olive oil
  10 gram salt
```

**Step 2: Click "Create Meal"**
✅ EDAMAM calculates nutrition automatically
✅ Displays in grid with calories & macros

### Analyze a Meal
1. Select meal from grid
2. Click "Analyze"
3. View detailed nutrition breakdown

### Delete a Meal
1. Select meal from grid
2. Click "Delete Selected"
3. Confirm deletion

### View Daily Summary
1. Pick a date
2. Click "Daily Summary"
3. See aggregate nutrition for that day

---

## 📊 Nutrition Data Provided

- 🔥 Calories (kcal)
- 💪 Protein (g)
- 🌾 Carbohydrates (g)
- 🧈 Fat (g)
- 🧂 Sodium (mg)
- 🍬 Sugar (g)
- 🥩 Saturated Fat (g)

---

## 🎨 UI Colors

| Color | Meaning |
|-------|---------|
| 🟢 Green | Create/Positive |
| 🔵 Blue | Analyze/Info |
| 🔴 Red | Delete/Warning |
| ⚫ Gray | Neutral/Refresh |
| 🟠 Orange | Summary |

---

## ⚙️ Status Bar Messages

| Message | Status |
|---------|--------|
| `Ready` | App idle, ready to use |
| `Loaded X meals` | Meals successfully loaded |
| `Analyzing meal...` | API call in progress |
| `✓ Meal created with Y calories` | Success! |
| `Meal deleted` | Deletion successful |

---

## 🔧 Troubleshooting

| Problem | Fix |
|---------|-----|
| **App won't start** | Check EDAMAM_APP_ID and EDAMAM_APP_KEY env vars |
| **Grid empty** | Click "Refresh" button |
| **Slow creation** | Check internet connection |
| **Data not saving** | Check `%APPDATA%\MealTracker\` folder |

---

## 📝 Ingredient Format

```
<quantity> <unit> <ingredient_name>
```

### Examples:
```
200 gram chicken breast
250 ml milk
1 cup rice
50 g olive oil
2 tablespoon honey
500 ml water
```

---

## 💾 Where Data is Stored

**Database**: `%APPDATA%\MealTracker\meals.db`
- Auto-created on first meal
- LiteDB format
- Local storage, no cloud sync

---

## 🚀 Performance

| Operation | Time |
|-----------|------|
| Create meal | <1 second |
| Load meals | ~500ms |
| Delete meal | ~300ms |
| Daily summary | ~400ms |

---

## 📱 Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `Tab` | Move between fields |
| `Enter` | Submit (in input field) |
| `Delete` | Delete selected meal |
| `F5` | Refresh grid |

---

## ❌ Common Mistakes

❌ Forgetting to set environment variables
✅ Do: `$env:EDAMAM_APP_ID = "..."`

❌ Wrong ingredient format
✅ Do: `200 gram chicken` 
❌ Don't: `chicken, 200g`

❌ Leaving ingredients empty
✅ Do: Add at least one ingredient
❌ Don't: Leave "Recipes & Ingredients" blank

---

## ✅ Checklist

- [ ] EDAMAM account created
- [ ] App ID and App Key copied
- [ ] Environment variables set
- [ ] Build successful
- [ ] App launches without errors
- [ ] Can create a meal
- [ ] Nutritional data displays
- [ ] Can delete meals
- [ ] Daily summary works

---

## 📞 Support

- **EDAMAM API Docs**: https://developer.edamam.com/docs
- **LiteDB Docs**: https://www.litedb.org/
- **Report Issues**: Check the error message in status bar

---

## 💡 Tips & Tricks

**Tip 1**: Use copy-paste for ingredients
- Enter once, paste multiple times

**Tip 2**: Breakfast meals are pre-selected
- Reduces clicking for morning meals

**Tip 3**: Grid shows 8 columns
- Scroll right to see all nutrition data

**Tip 4**: Dates default to today
- Change if entering past meals

**Tip 5**: Status bar gives real-time feedback
- Watch for completion messages

---

## 🎓 Learning the API

EDAMAM free tier:
- 60 requests/minute
- 500 requests/day
- Specialized nutrition database
- Instant lookups

---

**Happy Meal Tracking!** 🍽️
