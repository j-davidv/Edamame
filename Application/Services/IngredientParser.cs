using System.Text.RegularExpressions;
using Edamam.Domain.Entities;
using Edamam.Application.Interfaces;

namespace Edamam.Application.Services;

public class IngredientParser : IIngredientParser
{
    private readonly Dictionary<string, string> _unitAbbreviations = new(StringComparer.OrdinalIgnoreCase)
    {
        // Weight
        { "kg", "kilogram" },
        { "g", "gram" },
        { "mg", "milligram" },
        { "lb", "pound" },
        { "lbs", "pound" },
        { "oz", "ounce" },
        { "oz.", "ounce" },

        // Volume
        { "ml", "milliliter" },
        { "l", "liter" },
        { "cl", "centiliter" },
        { "cup", "cup" },
        { "tbsp", "tablespoon" },
        { "tsp", "teaspoon" },
        { "fl oz", "fluid ounce" },

        // Others
        { "pcs", "piece" },
        { "pc", "piece" },
        { "pcs.", "piece" },
        { "count", "piece" },
    };

    /// parse single ingredient
    public Ingredient? Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var trimmedInput = input.Trim();

        // pattern 1
        var ingredient = TryParseQuantityFirst(trimmedInput);
        if (ingredient != null)
            return ingredient;

        // pattern 2
        ingredient = TryParseQuantityLast(trimmedInput);
        if (ingredient != null)
            return ingredient;

        // pattern 3
        ingredient = TryParseNameOnly(trimmedInput);
        if (ingredient != null)
            return ingredient;

        return null;
    }

    /// parse multiple lines at once
    public List<Ingredient> ParseMultiple(string multiLineText)
    {
        if (string.IsNullOrWhiteSpace(multiLineText))
            return new List<Ingredient>();

        return multiLineText
            .Split('\n')
            .Select(line => Parse(line.Trim()))
            .Where(ingredient => ingredient != null)
            .Cast<Ingredient>()
            .ToList();
    }

    private Ingredient? TryParseQuantityFirst(string input)
    {
        // patterns like: "1kg", "1 kg", "1.5 kilogram", "2 cups"
        var pattern = @"^(?<quantity>[\d.]+)\s*(?<unit>[a-zA-Z]+\.?)\s+(?<name>.+)$";
        var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

        if (!match.Success)
            return null;

        if (!decimal.TryParse(match.Groups["quantity"].Value, out var quantity))
            return null;

        var unit = NormalizeUnit(match.Groups["unit"].Value);
        var name = match.Groups["name"].Value.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return null;

        try
        {
            return new Ingredient(name, quantity, unit);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private Ingredient? TryParseQuantityLast(string input)
    {
        // patterns like: "chicken breast 1kg", "flour 2 cups"
        var pattern = @"^(?<name>.+?)\s+(?<quantity>[\d.]+)\s*(?<unit>[a-zA-Z]+\.?)$";
        var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

        if (!match.Success)
            return null;

        if (!decimal.TryParse(match.Groups["quantity"].Value, out var quantity))
            return null;

        var name = match.Groups["name"].Value.Trim();
        var unit = NormalizeUnit(match.Groups["unit"].Value);

        if (string.IsNullOrWhiteSpace(name))
            return null;

        try
        {
            return new Ingredient(name, quantity, unit);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private Ingredient? TryParseNameOnly(string input)
    {
        // patterns like: "2 chicken breasts", "3 apples"
        var pattern = @"^(?<quantity>[\d.]+)\s+(?<name>.+)$";
        var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

        if (!match.Success)
            return null;

        if (!decimal.TryParse(match.Groups["quantity"].Value, out var quantity))
            return null;

        var name = match.Groups["name"].Value.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return null;

        try
        {
            return new Ingredient(name, quantity, "piece");
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private string NormalizeUnit(string unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
            return "piece";

        var trimmedUnit = unit.Trim().ToLowerInvariant();

        if (_unitAbbreviations.TryGetValue(trimmedUnit, out var fullUnit))
            return fullUnit;

        return trimmedUnit;
    }

    /// check if text looks like an ingredient line
    public bool LooksLikeIngredient(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        // Check if input contains any digit
        return Regex.IsMatch(input, @"\d");
    }
}

