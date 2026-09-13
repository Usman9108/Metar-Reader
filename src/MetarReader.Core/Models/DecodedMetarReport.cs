namespace MetarReader.Core.Models;

/// <summary>The plain-English translation of a <see cref="MetarObservation"/>.</summary>
public sealed record DecodedMetarReport(
    string StationName,
    string Summary,
    IReadOnlyList<string> Details,
    string RawMetar,
    DateTimeOffset ObservedAtUtc);
