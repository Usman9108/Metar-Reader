using MetarReader.Core.Weather;
using Xunit;

namespace MetarReader.Tests;

public class FlightCategoryClassifierTests
{
    [Theory]
    [InlineData(null, null, FlightCategory.Vfr)]
    [InlineData(5000, 10.0, FlightCategory.Vfr)]
    [InlineData(2000, 10.0, FlightCategory.Mvfr)]
    [InlineData(700, 10.0, FlightCategory.Ifr)]
    [InlineData(300, 10.0, FlightCategory.Lifr)]
    [InlineData(null, 4.0, FlightCategory.Mvfr)]
    [InlineData(null, 2.0, FlightCategory.Ifr)]
    [InlineData(null, 0.5, FlightCategory.Lifr)]
    [InlineData(5000, 0.5, FlightCategory.Lifr)] // visibility is worse than ceiling -> visibility governs
    public void Classify_ReturnsWorseOfCeilingAndVisibility(int? ceilingFeet, double? visibilityMiles, FlightCategory expected)
    {
        Assert.Equal(expected, FlightCategoryClassifier.Classify(ceilingFeet, visibilityMiles));
    }
}
