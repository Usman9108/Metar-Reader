using MetarReader.Core.Models;

namespace MetarReader.Core.Weather;

/// <summary>Translates the METAR cloud layer list into a plain-English sky description.</summary>
public static class SkyConditionDecoder
{
    /// <summary>Coverage codes that mean a clear sky rather than an actual cloud layer.</summary>
    private static readonly HashSet<string> ClearCodes = new(StringComparer.OrdinalIgnoreCase) { "SKC", "CLR", "NCD" };

    /// <summary>
    /// Higher rank = more significant when several layers are reported; the
    /// most significant layer governs the overall sky description.
    /// </summary>
    private static readonly Dictionary<string, int> CoverRank = new(StringComparer.OrdinalIgnoreCase)
    {
        ["FEW"] = 1,
        ["SCT"] = 2,
        ["BKN"] = 3,
        ["OVC"] = 4,
        ["VV"] = 5,
    };

    /// <summary>
    /// The aviation "ceiling" — the base of the lowest broken/overcast/obscured layer,
    /// or null if the sky has no ceiling (clear, or only scattered/few clouds).
    /// </summary>
    /// <param name="clouds">The reported sky condition layers.</param>
    /// <returns>The ceiling height in feet, or null if there is no ceiling.</returns>
    public static int? GetCeilingFeet(IReadOnlyList<CloudLayer> clouds)
    {
        var ceilingLayerBases = clouds
            .Where(c => CoverRank.TryGetValue(c.Cover, out var rank) && rank >= CoverRank["BKN"] && c.BaseFeet.HasValue)
            .Select(c => c.BaseFeet!.Value)
            .ToList();

        return ceilingLayerBases.Count == 0 ? null : ceilingLayerBases.Min();
    }

    /// <summary>Describes the overall sky condition in plain English, e.g. "Overcast at 1,200 ft".</summary>
    /// <param name="clouds">The reported sky condition layers.</param>
    /// <returns>A plain-English sky description, driven by the most significant layer.</returns>
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
