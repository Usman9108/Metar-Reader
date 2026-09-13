namespace MetarReader.Core.Models;

/// <summary>One SKY_CONDITION layer reported in a METAR, e.g. "BKN" at 3,000 ft.</summary>
/// <param name="Cover">
/// The layer's coverage code: <c>FEW</c>, <c>SCT</c> (scattered), <c>BKN</c> (broken),
/// <c>OVC</c> (overcast), or <c>VV</c> (indefinite ceiling / sky obscured).
/// </param>
/// <param name="BaseFeet">The height of the layer's base above ground, in feet, or null if not reported.</param>
public sealed record CloudLayer(string Cover, int? BaseFeet);
