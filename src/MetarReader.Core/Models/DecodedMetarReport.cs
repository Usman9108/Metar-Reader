using MetarReader.Core.Weather;

namespace MetarReader.Core.Models;

/// <summary>
/// The plain-English translation of a <see cref="MetarObservation"/>, plus the
/// structured values the UI needs to draw a wind dial, data tiles, and the
/// FAA flight-category badge. Produced by <see cref="MetarDecoder.Decode"/>.
/// </summary>
/// <param name="StationName">The station's descriptive name, falling back to its ICAO id if none was reported.</param>
/// <param name="Summary">A one-sentence, plain-English weather summary, e.g. "Clear skies, 60°F, wind 4 mph from the north."</param>
/// <param name="Details">Supplementary plain-English detail lines (visibility, weather, altimeter, observation time).</param>
/// <param name="RawMetar">The original, unmodified METAR text.</param>
/// <param name="ObservedAtUtc">The instant the observation was taken.</param>
/// <param name="FlightCategory">The FAA flight category (VFR/MVFR/IFR/LIFR) derived from ceiling and visibility.</param>
/// <param name="SkyPhrase">The plain-English sky condition alone, e.g. "Overcast at 1,200 ft".</param>
/// <param name="TempF">Air temperature in degrees Fahrenheit, or null if not reported.</param>
/// <param name="WindDirectionDegrees">Wind direction in degrees true, or null if calm or variable.</param>
/// <param name="IsVariableWind">True when the wind direction is variable ("VRB").</param>
/// <param name="WindSpeedMph">Sustained wind speed in miles per hour.</param>
/// <param name="WindGustMph">Gust speed in miles per hour, or null if no gust was reported.</param>
/// <param name="VisibilityMiles">Visibility in statute miles, or null if unparseable.</param>
/// <param name="VisibilityPhrase">Visibility rendered as a plain-English phrase, e.g. "10 miles or more".</param>
/// <param name="CeilingFeet">
/// The aviation ceiling in feet — the base of the lowest broken/overcast/obscured layer — or null if there is no ceiling.
/// </param>
/// <param name="AltimeterInHg">Altimeter setting in inches of mercury, or null if not reported.</param>
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
