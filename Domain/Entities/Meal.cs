using LiteDB;

namespace Edamam.Domain.Entities;

public class Meal : EntityBase
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Meal name cannot be empty", nameof(value));
            _name = value.Trim();
        }
    }

    public string? Notes { get; set; }

    private readonly List<Recipe> _recipes = new();
    public IReadOnlyList<Recipe> Recipes => _recipes.AsReadOnly();

    public NutritionalMetric? Nutritionals { get; set; }

    public DateTime MealDate { get; set; } = DateTime.UtcNow;

    private MealType _type = MealType.Lunch;
    public MealType Type
    {
        get => _type;
        set
        {
            if (!Enum.IsDefined(typeof(MealType), value))
                throw new ArgumentException($"Invalid meal type: {value}", nameof(value));
            _type = value;
        }
    }

    public Meal(string name, MealType type = MealType.Lunch, DateTime? mealDate = null)
    {
        Name = name;  // Triggers validation
        Type = type;  // Triggers validation
        MealDate = mealDate ?? DateTime.UtcNow;
        ValidateState();  
    }

    public Meal() { }

    public override void ValidateState()
    {
        if (string.IsNullOrWhiteSpace(_name))
            throw new InvalidOperationException("Meal must have a valid name");

        if (!Enum.IsDefined(typeof(MealType), _type))
            throw new InvalidOperationException($"Meal has invalid type: {_type}");

        // validate nutritional data if present
        if (Nutritionals != null)
        {
            Nutritionals.ValidateState();
        }

        // Validate all recipes
        foreach (var recipe in _recipes)
        {
            recipe.ValidateState();
        }
    }

    /// get display name for the meal
    public override string GetDisplayName()
    {
        return $"{Name} ({Type}) - {MealDate:g}";
    }

    /// add recipe with virtual method
    public virtual void AddRecipe(Recipe recipe)
    {
        if (recipe == null)
            throw new ArgumentNullException(nameof(recipe), "Recipe cannot be null");
        recipe.ValidateState();
        _recipes.Add(recipe);
        MarkAsModified();
    }

    /// add multiple recipes at once
    public void AddRecipes(IEnumerable<Recipe> recipes)
    {
        if (recipes == null)
            throw new ArgumentNullException(nameof(recipes));
        
        foreach (var recipe in recipes)
        {
            AddRecipe(recipe);
        }
    }

    /// Remove recipe by ID
    public bool RemoveRecipe(ObjectId recipeId)
    {
        var removed = _recipes.RemoveAll(r => r.Id == recipeId) > 0;
        if (removed)
            MarkAsModified();
        return removed;
    }

    /// clear all recipes 
    public virtual void ClearRecipes()
    {
        if (_recipes.Count > 0)
        {
            _recipes.Clear();
            MarkAsModified();
        }
    }

    public override string ToString() => $"Meal: {Name} ({_recipes.Count} recipes) - {MealDate:g}";
}

/// enums for polymorphication support
public enum MealType
{
    Breakfast,
    Brunch,
    Lunch,
    Snack,
    Dinner,
    Supper
}
