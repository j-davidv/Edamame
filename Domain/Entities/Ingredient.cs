using LiteDB;

namespace Edamam.Domain.Entities;

public class Ingredient : EntityBase
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ingredient name cannot be empty or whitespace", nameof(value));
            _name = value.Trim();
        }
    }

    private decimal _quantity;
    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(value));
            _quantity = value;
        }
    }

    private string _unit = string.Empty;
    public string Unit
    {
        get => _unit;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Unit cannot be empty or whitespace", nameof(value));
            _unit = value.Trim();
        }
    }

    // Constructor for validation on creation
    public Ingredient(string name, decimal quantity, string unit)
    {
        Name = name;      // Triggers validation
        Quantity = quantity;  // Triggers validation
        Unit = unit;      // Triggers validation
        ValidateState();  
    }

    // parameterless constructor for liteDB 
    public Ingredient() { }

    /// validate the complete state of the ingredient
    public override void ValidateState()
    {
        if (string.IsNullOrWhiteSpace(_name))
            throw new InvalidOperationException("Ingredient must have a valid name");

        if (_quantity <= 0)
            throw new InvalidOperationException("Ingredient quantity must be greater than zero");

        if (string.IsNullOrWhiteSpace(_unit))
            throw new InvalidOperationException("Ingredient must have a valid unit");
    }

    /// get display name for the ingredient
    public override string GetDisplayName()
    {
        return $"{_quantity} {_unit} {_name}";
    }

    public override string ToString() => $"{Quantity} {Unit} {Name}";
}
