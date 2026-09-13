namespace MetarReader.Core.Models;

/// <summary>
/// Normalized representation of a single METAR observation returned by the
/// aviationweather.gov "format=json" data API. Ambiguous API fields (wind
/// direction, visibility) are already resolved into typed values here.
/// </summary>
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
