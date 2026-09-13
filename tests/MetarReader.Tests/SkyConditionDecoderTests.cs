using MetarReader.Core.Models;
using MetarReader.Core.Weather;
using Xunit;

namespace MetarReader.Tests;

public class SkyConditionDecoderTests
{
    [Fact]
    public void Describe_NoLayers_ReturnsClearSkies()
    {
        Assert.Equal("Clear skies", SkyConditionDecoder.Describe([]));
    }

    [Fact]
    public void Describe_ClrOnly_ReturnsClearSkies()
    {
        Assert.Equal("Clear skies", SkyConditionDecoder.Describe([new CloudLayer("CLR", null)]));
    }

    [Fact]
    public void Describe_Few_ReturnsAFewClouds()
    {
        var result = SkyConditionDecoder.Describe([new CloudLayer("FEW", 25000)]);
        Assert.Equal("A few clouds at 25,000 ft", result);
    }

    [Fact]
    public void Describe_PicksMostSignificantLayer()
    {
        var clouds = new[]
        {
            new CloudLayer("FEW", 3000),
            new CloudLayer("BKN", 8000),
            new CloudLayer("SCT", 5000),
        };

        Assert.Equal("Mostly cloudy at 8,000 ft", SkyConditionDecoder.Describe(clouds));
    }

    [Fact]
    public void Describe_Overcast_ReturnsOvercast()
    {
        Assert.Equal("Overcast at 3,000 ft", SkyConditionDecoder.Describe([new CloudLayer("OVC", 3000)]));
    }

    [Fact]
    public void Describe_VerticalVisibility_ReturnsSkyObscured()
    {
        Assert.Equal(
            "Sky obscured (vertical visibility 500 ft)",
            SkyConditionDecoder.Describe([new CloudLayer("VV", 500)]));
    }
}
