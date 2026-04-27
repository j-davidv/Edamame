using LiteDB;

namespace Edamam.Domain.Entities;

public class Recipe : EntityBase
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Recipe name cannot be empty", nameof(value));
            _name = value.Trim();
        }
    }

    public string? Description { get; set; }

    private readonly List<Ingredient> _ingredients = new();
    public IReadOnlyList<Ingredient> Ingredients => _ingredients.AsReadOnly();

    public Recipe(string name, string? description = null)
    {
        Name = name;  // Triggers validation
        Description = description;
        ValidateState();  
    }

    public Recipe() { }

    /// check complete state of the recipe
    public override void ValidateState()
    {
        if (string.IsNullOrWhiteSpace(_name))
            throw new InvalidOperationException("Recipe must have a valid name");

        // Validate all ingredients
        foreach (var ingredient in _ingredients)
        {
            ingredient.ValidateState();
        }
    }

    /// get display name for the recipe
    public override string GetDisplayName()
    {
        return $"{Name} ({_ingredients.Count} ingredients)";
    }

    /// add ingredient
    public virtual void AddIngredient(Ingredient ingredient)
    {
        if (ingredient == null)
            throw new ArgumentNullException(nameof(ingredient), "Ingredient cannot be null");
        ingredient.ValidateState();
        _ingredients.Add(ingredient);
        MarkAsModified();
    }

    /// add multiple ingredients at once
    public void AddIngredients(IEnumerable<Ingredient> ingredients)
    {
        if (ingredients == null)
            throw new ArgumentNullException(nameof(ingredients));
        
        foreach (var ingredient in ingredients)
        {
            AddIngredient(ingredient);
        }
    }

    /// remove ingredient by ID
    public bool RemoveIngredient(ObjectId ingredientId)
    {
        var removed = _ingredients.RemoveAll(i => i.Id == ingredientId) > 0;
        if (removed)
            MarkAsModified();
        return removed;
    }

    /// clear all ingredients
    public virtual void ClearIngredients()
    {
        if (_ingredients.Count > 0)
        {
            _ingredients.Clear();
            MarkAsModified();
        }
    }

    public override string ToString() => $"Recipe: {Name} ({_ingredients.Count} ingredients)";
}


