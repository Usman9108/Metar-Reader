using MetarReader.Core.Models;

namespace MetarReader.Core.Weather;

/// <summary>Translates the METAR cloud layer list into a plain-English sky description.</summary>
public static class SkyConditionDecoder
{
    private static readonly HashSet<string> ClearCodes = new(StringComparer.OrdinalIgnoreCase) { "SKC", "CLR", "NCD" };

    // Higher rank = more significant when several layers are reported; the
    // most significant layer governs the overall sky description.
    private static readonly Dictionary<string, int> CoverRank = new(StringComparer.OrdinalIgnoreCase)
    {
        ["FEW"] = 1,
        ["SCT"] = 2,
        ["BKN"] = 3,
        ["OVC"] = 4,
        ["VV"] = 5,
    };

    public static string Describe(IReadOnlyList<CloudLayer> clouds)
    {
        var significant = clouds
            .Where(c => !ClearCodes.Contains(c.Cover) && CoverRank.ContainsKey(c.Cover))
            .ToList();

        if (significant.Count == 0)
        {
            return "Clear skies";
        }

        var governing = significant
            .OrderByDescending(c => CoverRank[c.Cover])
            .ThenBy(c => c.BaseFeet ?? int.MaxValue)
            .First();

        var baseText = governing.BaseFeet is { } feet ? $" at {feet:N0} ft" : string.Empty;

        return governing.Cover.ToUpperInvariant() switch
        {
            "FEW" => $"A few clouds{baseText}",
            "SCT" => $"Partly cloudy{baseText}",
            "BKN" => $"Mostly cloudy{baseText}",
            "OVC" => $"Overcast{baseText}",
            "VV" => $"Sky obscured{(governing.BaseFeet is { } vv ? $" (vertical visibility {vv:N0} ft)" : string.Empty)}",
            _ => "Clear skies",
        };
    }
}
