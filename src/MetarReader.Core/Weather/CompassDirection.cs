namespace MetarReader.Core.Weather;

public static class CompassDirection
{
    private static readonly string[] Points =
    [
        "north", "north-northeast", "northeast", "east-northeast",
        "east", "east-southeast", "southeast", "south-southeast",
        "south", "south-southwest", "southwest", "west-southwest",
        "west", "west-northwest", "northwest", "north-northwest",
    ];

    /// <summary>Converts a wind direction in degrees (0-360) to a 16-point compass name.</summary>
    public static string FromDegrees(int degrees)
    {
        var normalized = ((degrees % 360) + 360) % 360;
        var index = (int)Math.Round(normalized / 22.5, MidpointRounding.AwayFromZero) % 16;
        return Points[index];
    }
}
