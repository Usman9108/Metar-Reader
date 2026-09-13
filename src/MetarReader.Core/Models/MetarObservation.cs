namespace MetarReader.Core.Models;

/// <summary>
/// Normalized representation of a single METAR observation returned by the
/// aviationweather.gov "format=json" data API. Ambiguous API fields (wind
/// direction, visibility) are already resolved into typed values here.
/// </summary>
/// <param name="IcaoId">The station's four-letter ICAO identifier, e.g. "KHIO".</param>
/// <param name="StationName">The station's descriptive name, or null if the API didn't report one.</param>
/// <param name="ObservedAtUtc">The instant the observation was taken.</param>
/// <param name="TempC">Air temperature in degrees Celsius, or null if not reported.</param>
/// <param name="DewpointC">Dewpoint in degrees Celsius, or null if not reported.</param>
/// <param name="WindDirectionDegrees">Wind direction in degrees true, or null if calm or variable.</param>
/// <param name="IsVariableWind">True when the METAR reported a variable ("VRB") wind direction.</param>
/// <param name="WindSpeedKt">Sustained wind speed in knots, or null if not reported.</param>
/// <param name="WindGustKt">Gust speed in knots, or null if no gust was reported.</param>
/// <param name="VisibilityRaw">
/// The raw visibility value as returned by the API (e.g. "10+", "1/2", "1 1/4"), or null if not reported.
/// See <see cref="Weather.VisibilityParser"/> for how this is interpreted.
/// </param>
/// <param name="Clouds">The reported sky condition layers, lowest first.</param>
/// <param name="WxString">The raw present-weather codes (e.g. "-RA BR"), or null if none were reported.</param>
/// <param name="AltimeterHpa">Altimeter setting in hectopascals, or null if not reported.</param>
/// <param name="RawText">The original, unmodified METAR text.</param>
public sealed record MetarObservation(
    string IcaoId,
    string? StationName,
    DateTimeOffset ObservedAtUtc,
    double? TempC,
    double? DewpointC,
    int? WindDirectionDegrees,
    bool IsVariableWind,
    int? WindSpeedKt,
    int? WindGustKt,
    string? VisibilityRaw,
    IReadOnlyList<CloudLayer> Clouds,
    string? WxString,
    double? AltimeterHpa,
    string RawText);
