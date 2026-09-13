using MetarReader.Core.Weather;
using Xunit;

namespace MetarReader.Tests;

public class CompassDirectionTests
{
    [Theory]
    [InlineData(0, "north")]
    [InlineData(90, "east")]
    [InlineData(180, "south")]
    [InlineData(270, "west")]
    [InlineData(320, "northwest")]
    [InlineData(360, "north")]
    [InlineData(10, "north")]
    public void FromDegrees_ReturnsExpectedCompassPoint(int degrees, string expected)
    {
        Assert.Equal(expected, CompassDirection.FromDegrees(degrees));
    }
}
