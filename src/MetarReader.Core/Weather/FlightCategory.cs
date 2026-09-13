namespace MetarReader.Core.Weather;

/// <summary>
/// The FAA flight category scale pilots use to judge whether conditions
/// support visual flight. Ordered best-to-worst so the numeric value can be
/// compared directly.
/// </summary>
public enum FlightCategory
{
    Vfr = 0,
    Mvfr = 1,
    Ifr = 2,
    Lifr = 3,
}

/// <summary>
/// Classifies a flight category from ceiling and visibility using the
/// standard FAA thresholds. Whichever of the two yields the worse category governs.
/// </summary>
public static class FlightCategoryClassifier
{
    public static FlightCategory Classify(int? ceilingFeet, double? visibilityMiles)
    {
        var byCeiling = ceilingFeet switch
        {
            null => FlightCategory.Vfr,
            <= 499 => FlightCategory.Lifr,
            <= 999 => FlightCategory.Ifr,
            <= 3000 => FlightCategory.Mvfr,
            _ => FlightCategory.Vfr,
        };

        var byVisibility = visibilityMiles switch
        {
            null => FlightCategory.Vfr,
            < 1 => FlightCategory.Lifr,
            < 3 => FlightCategory.Ifr,
            <= 5 => FlightCategory.Mvfr,
            _ => FlightCategory.Vfr,
        };

        return byCeiling > byVisibility ? byCeiling : byVisibility;
    }
}
