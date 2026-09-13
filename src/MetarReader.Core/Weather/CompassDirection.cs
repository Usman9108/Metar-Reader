namespace MetarReader.Core.Weather;

/// <summary>Converts a wind direction in degrees into a plain-English compass point.</summary>
public static class CompassDirection
{
    /// <summary>The 16 compass points, in order starting from north, spaced 22.5 degrees apart.</summary>
    private static readonly string[] Points =
    [
        "north", "north-northeast", "northeast", "east-northeast",
        "east", "east-southeast", "southeast", "south-southeast",
        "south", "south-southwest", "southwest", "west-southwest",
        "west", "west-northwest", "northwest", "north-northwest",
    ];

    /// <summary>Converts a wind direction in degrees (0-360) to a 16-point compass name.</summary>
    /// <param name="degrees">The wind direction in degrees true. Values outside 0-360 are normalized.</param>
    /// <returns>The nearest 16-point compass name, e.g. "northwest".</returns>
    public static string FromDegrees(int degrees)
    {
        var normalized = ((degrees % 360) + 360) % 360;
        var index = (int)Math.Round(normalized / 22.5, MidpointRounding.AwayFromZero) % 16;
        return Points[index];
    }
}
