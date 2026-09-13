using MetarReader.Core.Models;
using MetarReader.Core.Weather;
using Xunit;

namespace MetarReader.Tests;

public class MetarDecoderTests
{
    private static readonly DateTimeOffset ObservedAt = new(2024, 1, 1, 18, 55, 0, TimeSpan.Zero);

    [Fact]
    public void Decode_ClearCalmDay_MatchesExpectedSummary()
    {
        var obs = new MetarObservation(
            "KHIO", "Portland-Hillsboro Airport", ObservedAt,
            TempC: 15.6, DewpointC: 10.0,
            WindDirectionDegrees: 320, IsVariableWind: false,
            WindSpeedKt: 4, WindGustKt: null,
            VisibilityRaw: "10+",
            Clouds: [],
            WxString: null,
            AltimeterHpa: 1019.3,
            RawText: "KHIO 011855Z 32004KT 10SM CLR 16/10 A3010");

        var report = MetarDecoder.Decode(obs);

        Assert.Equal("Clear skies, 60°F, wind 5 mph from the northwest.", report.Summary);
        Assert.Contains("Visibility: 10 miles or more", report.Details);
        Assert.Contains(report.Details, d => d.StartsWith("Altimeter: 30.10 inHg"));
        Assert.Equal("KHIO 011855Z 32004KT 10SM CLR 16/10 A3010", report.RawMetar);
    }

    [Fact]
    public void Decode_CalmWind_ReturnsCalmWinds()
    {
        var obs = new MetarObservation(
            "KHIO", null, ObservedAt,
            TempC: 20, DewpointC: 10,
            WindDirectionDegrees: null, IsVariableWind: false,
            WindSpeedKt: 0, WindGustKt: null,
            VisibilityRaw: "10",
            Clouds: [],
            WxString: null,
            AltimeterHpa: null,
            RawText: "raw");

        var report = MetarDecoder.Decode(obs);

        Assert.Equal("Clear skies, 68°F, calm winds.", report.Summary);
    }

    [Fact]
    public void Decode_GustingWindWithRainAndOvercast_IncludesAllElements()
    {
        var obs = new MetarObservation(
            "KJFK", "John F. Kennedy International Airport", ObservedAt,
            TempC: 10, DewpointC: 8,
            WindDirectionDegrees: 180, IsVariableWind: false,
            WindSpeedKt: 12, WindGustKt: 20,
            VisibilityRaw: "3",
            Clouds: [new CloudLayer("OVC", 1200)],
            WxString: "-RA",
            AltimeterHpa: 1000,
            RawText: "raw");

        var report = MetarDecoder.Decode(obs);

        Assert.Equal(
            "Overcast at 1,200 ft with light rain, 50°F, wind 14 mph from the south, gusting to 23 mph.",
            report.Summary);
        Assert.Contains("Weather: light rain", report.Details);
    }

    [Fact]
    public void Decode_VariableWind_DoesNotIncludeCompassDirection()
    {
        var obs = new MetarObservation(
            "KABQ", null, ObservedAt,
            TempC: 25, DewpointC: 5,
            WindDirectionDegrees: null, IsVariableWind: true,
            WindSpeedKt: 5, WindGustKt: null,
            VisibilityRaw: "10+",
            Clouds: [],
            WxString: null,
            AltimeterHpa: null,
            RawText: "raw");

        var report = MetarDecoder.Decode(obs);

        Assert.Contains("wind 6 mph variable direction", report.Summary);
    }

    [Fact]
    public void Decode_MissingStationName_FallsBackToIcaoId()
    {
        var obs = new MetarObservation(
            "KABQ", null, ObservedAt,
            TempC: null, DewpointC: null,
            WindDirectionDegrees: null, IsVariableWind: false,
            WindSpeedKt: null, WindGustKt: null,
            VisibilityRaw: null,
            Clouds: [],
            WxString: null,
            AltimeterHpa: null,
            RawText: "raw");

        var report = MetarDecoder.Decode(obs);

        Assert.Equal("KABQ", report.StationName);
        Assert.Equal("Clear skies, calm winds.", report.Summary);
    }
}
