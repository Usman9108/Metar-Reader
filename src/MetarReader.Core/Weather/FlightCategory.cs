namespace MetarReader.Core.Weather;

/// <summary>
/// The FAA flight category scale pilots use to judge whether conditions
/// support visual flight. Ordered best-to-worst so the numeric value can be
/// compared directly.
/// </summary>
public enum FlightCategory
{
    /// <summary>Visual Flight Rules: ceiling greater than 3,000 ft and visibility greater than 5 miles.</summary>
    Vfr = 0,

    /// <summary>Marginal VFR: ceiling 1,000-3,000 ft and/or visibility 3-5 miles.</summary>
    Mvfr = 1,

    /// <summary>Instrument Flight Rules: ceiling 500-999 ft and/or visibility 1-2.9 miles.</summary>
    Ifr = 2,

    /// <summary>Low IFR: ceiling below 500 ft and/or visibility below 1 mile.</summary>
    Lifr = 3,
}

/// <summary>
/// Classifies a flight category from ceiling and visibility using the
/// standard FAA thresholds. Whichever of the two yields the worse category governs.
/// </summary>
public static class FlightCategoryClassifier
{
    /// <summary>Determines the flight category for a given ceiling and visibility.</summary>
    /// <param name="ceilingFeet">
    /// The aviation ceiling in feet (see <see cref="SkyConditionDecoder.GetCeilingFeet"/>), or null if there is no ceiling.
    /// </param>
    /// <param name="visibilityMiles">The visibility in statute miles, or null if unknown.</param>
    /// <returns>The worse (more restrictive) of the categories implied by ceiling and by visibility.</returns>
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
