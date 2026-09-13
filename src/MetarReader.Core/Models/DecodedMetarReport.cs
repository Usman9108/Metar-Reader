using MetarReader.Core.Weather;

namespace MetarReader.Core.Models;

/// <summary>
/// The plain-English translation of a <see cref="MetarObservation"/>, plus the
/// structured values the UI needs to draw a wind dial, data tiles, and the
/// FAA flight-category badge.
/// </summary>
public sealed record DecodedMetarReport(
    string StationName,
    string Summary,
    IReadOnlyList<string> Details,
    string RawMetar,
    DateTimeOffset ObservedAtUtc,
    FlightCategory FlightCategory,
    string SkyPhrase,
    int? TempF,
    int? WindDirectionDegrees,
    bool IsVariableWind,
    int WindSpeedMph,
    int? WindGustMph,
    double? VisibilityMiles,
    string VisibilityPhrase,
    int? CeilingFeet,
    double? AltimeterInHg);
