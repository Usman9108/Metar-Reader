using System.Globalization;
using MetarReader.Core.Models;

namespace MetarReader.Core.Weather;

/// <summary>Composes the individual decoders into a full plain-English report.</summary>
public static class MetarDecoder
{
    public static DecodedMetarReport Decode(MetarObservation obs)
    {
        var wxPhrases = WeatherPhenomenaDecoder.Decode(obs.WxString);

        var skyPhrase = SkyConditionDecoder.Describe(obs.Clouds);
        if (wxPhrases.Count > 0)
        {
            skyPhrase = $"{skyPhrase} with {JoinNatural(wxPhrases)}";
        }

        var summaryParts = new List<string> { skyPhrase };

        if (obs.TempC is { } tempC)
        {
            summaryParts.Add($"{UnitConversions.CelsiusToFahrenheit(tempC)}°F");
        }

        summaryParts.Add(BuildWindPhrase(obs));

        var summary = string.Join(", ", summaryParts) + ".";

        var details = BuildDetails(obs, wxPhrases);

        var stationName = string.IsNullOrWhiteSpace(obs.StationName) ? obs.IcaoId : obs.StationName!;

        return new DecodedMetarReport(stationName, summary, details, obs.RawText, obs.ObservedAtUtc);
    }

    private static string BuildWindPhrase(MetarObservation obs)
    {
        if ((obs.WindSpeedKt ?? 0) == 0)
        {
            return "calm winds";
        }

        var mph = UnitConversions.KnotsToMph(obs.WindSpeedKt!.Value);
        var direction = obs.IsVariableWind || obs.WindDirectionDegrees is null
            ? "variable direction"
            : $"from the {CompassDirection.FromDegrees(obs.WindDirectionDegrees.Value)}";

        var phrase = $"wind {mph} mph {direction}";

        if (obs.WindGustKt is { } gustKt)
        {
            phrase += $", gusting to {UnitConversions.KnotsToMph(gustKt)} mph";
        }

        return phrase;
    }

    private static List<string> BuildDetails(MetarObservation obs, IReadOnlyList<string> wxPhrases)
    {
        var details = new List<string>
        {
            $"Visibility: {VisibilityParser.Parse(obs.VisibilityRaw)}",
        };

        if (wxPhrases.Count > 0)
        {
            details.Add($"Weather: {JoinNatural(wxPhrases)}");
        }

        if (obs.AltimeterHpa is { } hpa)
        {
            var inHg = UnitConversions.HectopascalsToInchesOfMercury(hpa);
            details.Add($"Altimeter: {inHg.ToString("F2", CultureInfo.InvariantCulture)} inHg ({hpa:N0} hPa)");
        }

        details.Add($"Observed: {obs.ObservedAtUtc:yyyy-MM-dd HH:mm} UTC");

        return details;
    }

    private static string JoinNatural(IReadOnlyList<string> items) => items.Count switch
    {
        0 => string.Empty,
        1 => items[0],
        2 => $"{items[0]} and {items[1]}",
        _ => string.Join(", ", items.Take(items.Count - 1)) + $", and {items[^1]}",
    };
}
