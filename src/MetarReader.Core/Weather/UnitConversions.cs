namespace MetarReader.Core.Weather;

/// <summary>Unit conversions between the metric units METAR reports use and the US units the UI displays.</summary>
public static class UnitConversions
{
    /// <summary>Converts a temperature from degrees Celsius to the nearest whole degree Fahrenheit.</summary>
    /// <param name="celsius">The temperature in degrees Celsius.</param>
    /// <returns>The temperature in degrees Fahrenheit, rounded to the nearest whole number.</returns>
    public static int CelsiusToFahrenheit(double celsius) =>
        (int)Math.Round(celsius * 9.0 / 5.0 + 32.0, MidpointRounding.AwayFromZero);

    /// <summary>Converts a speed from knots to the nearest whole mile per hour.</summary>
    /// <param name="knots">The speed in knots.</param>
    /// <returns>The speed in miles per hour, rounded to the nearest whole number.</returns>
    public static int KnotsToMph(double knots) =>
        (int)Math.Round(knots * 1.15078, MidpointRounding.AwayFromZero);

    /// <summary>Converts an atmospheric pressure from hectopascals to inches of mercury.</summary>
    /// <param name="hpa">The pressure in hectopascals.</param>
    /// <returns>The pressure in inches of mercury.</returns>
    public static double HectopascalsToInchesOfMercury(double hpa) => hpa / 33.8639;
}
