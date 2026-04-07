namespace Edamam.Domain.Entities;

/// base class for nutritional information
public abstract class NutritionalBase
{
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbohydrates { get; set; }
    public decimal Fat { get; set; }
    public decimal Sodium { get; set; }
    public decimal Sugar { get; set; }
    public decimal SaturatedFat { get; set; }


    /// chceks if they're non-negative

    protected virtual bool IsValid()
    {
        return Calories >= 0 && Protein >= 0 && Carbohydrates >= 0 && 
               Fat >= 0 && Sodium >= 0 && Sugar >= 0 && SaturatedFat >= 0;
    }
}
