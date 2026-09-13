using MetarReader.Core.Weather;
using Xunit;

namespace MetarReader.Tests;

public class WeatherPhenomenaDecoderTests
{
    [Fact]
    public void Decode_NullOrEmpty_ReturnsNoPhrases()
    {
        Assert.Empty(WeatherPhenomenaDecoder.Decode(null));
        Assert.Empty(WeatherPhenomenaDecoder.Decode(""));
    }

    [Theory]
    [InlineData("-RA", "light rain")]
    [InlineData("+RA", "heavy rain")]
    [InlineData("RA", "rain")]
    [InlineData("-TSRA", "light thunderstorms with rain")]
    [InlineData("+TSRA", "heavy thunderstorms with rain")]
    [InlineData("BR", "mist")]
    [InlineData("FG", "fog")]
    [InlineData("HZ", "haze")]
    [InlineData("SN", "snow")]
    [InlineData("VCSH", "nearby rain showers")]
    public void Decode_SingleToken_ReturnsExpectedPhrase(string wxString, string expected)
    {
        var phrases = WeatherPhenomenaDecoder.Decode(wxString);
        Assert.Single(phrases);
        Assert.Equal(expected, phrases[0]);
    }

    [Fact]
    public void Decode_MultipleTokens_ReturnsOnePhraseEach()
    {
        var phrases = WeatherPhenomenaDecoder.Decode("-RA BR");

        Assert.Equal(2, phrases.Count);
        Assert.Equal("light rain", phrases[0]);
        Assert.Equal("mist", phrases[1]);
    }
}
