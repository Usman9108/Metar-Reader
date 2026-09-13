namespace MetarReader.Core.Models;

/// <summary>One SKY_CONDITION layer, e.g. "BKN" at 3000 ft.</summary>
public sealed record CloudLayer(string Cover, int? BaseFeet);
