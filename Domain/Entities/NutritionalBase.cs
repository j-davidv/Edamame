namespace Edamam.Domain.Entities;

/// <summary>
/// Base class for nutritional information (Inheritance principle).
/// Encapsulates common nutritional properties.
/// </summary>
public abstract class NutritionalBase
{
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbohydrates { get; set; }
    public decimal Fat { get; set; }
    public decimal Sodium { get; set; }
    public decimal Sugar { get; set; }
    public decimal SaturatedFat { get; set; }

    /// <summary>
    /// Validates nutritional values to ensure they're non-negative.
    /// </summary>
    protected virtual bool IsValid()
    {
        return Calories >= 0 && Protein >= 0 && Carbohydrates >= 0 && 
               Fat >= 0 && Sodium >= 0 && Sugar >= 0 && SaturatedFat >= 0;
    }
}
