namespace MetarReader.Core.Models;

public sealed record Airport(string IcaoId, string City, string Name)
{
    public string DisplayName => $"{IcaoId} — {City} ({Name})";
}
