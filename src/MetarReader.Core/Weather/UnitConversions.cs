namespace MetarReader.Core.Weather;

public static class UnitConversions
{
    public static int CelsiusToFahrenheit(double celsius) =>
        (int)Math.Round(celsius * 9.0 / 5.0 + 32.0, MidpointRounding.AwayFromZero);

    public static int KnotsToMph(double knots) =>
        (int)Math.Round(knots * 1.15078, MidpointRounding.AwayFromZero);

    public static double HectopascalsToInchesOfMercury(double hpa) => hpa / 33.8639;
}
