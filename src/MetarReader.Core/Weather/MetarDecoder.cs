using System.Globalization;
using MetarReader.Core.Models;

namespace MetarReader.Core.Weather;

/// <summary>Composes the individual decoders into a full plain-English report.</summary>
public static class MetarDecoder
{
    /// <summary>
    /// Translates a raw <see cref="MetarObservation"/> into a <see cref="DecodedMetarReport"/>:
    /// a plain-English summary and details, plus the structured values (flight category, wind,
    /// ceiling, etc.) the UI needs to render them.
    /// </summary>
    /// <param name="obs">The observation to decode.</param>
    /// <returns>The decoded, plain-English report.</returns>
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

        var visibilityMiles = VisibilityParser.ParseMiles(obs.VisibilityRaw);
        var ceilingFeet = SkyConditionDecoder.GetCeilingFeet(obs.Clouds);
        var flightCategory = FlightCategoryClassifier.Classify(ceilingFeet, visibilityMiles);
        var windSpeedMph = UnitConversions.KnotsToMph(obs.WindSpeedKt ?? 0);

        return new DecodedMetarReport(
            stationName,
            summary,
            details,
            obs.RawText,
            obs.ObservedAtUtc,
            flightCategory,
            SkyConditionDecoder.Describe(obs.Clouds),
            obs.TempC is { } t ? UnitConversions.CelsiusToFahrenheit(t) : null,
            obs.WindDirectionDegrees,
            obs.IsVariableWind,
            windSpeedMph,
            obs.WindGustKt is { } g ? UnitConversions.KnotsToMph(g) : null,
            visibilityMiles,
            VisibilityParser.Parse(obs.VisibilityRaw),
            ceilingFeet,
            obs.AltimeterHpa is { } hpa ? UnitConversions.HectopascalsToInchesOfMercury(hpa) : null);
    }

    /// <summary>Builds the wind clause of the summary sentence, e.g. "wind 12 mph from the southeast, gusting to 20 mph".</summary>
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

    /// <summary>Builds the supplementary detail lines shown alongside the summary sentence.</summary>
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

    /// <summary>Joins a list of phrases with commas and a trailing "and", e.g. "rain, fog, and haze".</summary>
    private static string JoinNatural(IReadOnlyList<string> items) => items.Count switch
    {
        0 => string.Empty,
        1 => items[0],
        2 => $"{items[0]} and {items[1]}",
        _ => string.Join(", ", items.Take(items.Count - 1)) + $", and {items[^1]}",
    };
}
