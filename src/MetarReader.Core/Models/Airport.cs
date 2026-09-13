namespace MetarReader.Core.Models;

/// <summary>An airport that can be selected from the picker.</summary>
/// <param name="IcaoId">The airport's four-letter ICAO identifier, e.g. "KHIO".</param>
/// <param name="City">The city and, for US airports, state the airport serves, e.g. "Portland, OR".</param>
/// <param name="Name">The airport's full official name, e.g. "Portland-Hillsboro Airport".</param>
public sealed record Airport(string IcaoId, string City, string Name)
{
    /// <summary>A single string combining <see cref="IcaoId"/>, <see cref="City"/>, and <see cref="Name"/> for display in a list.</summary>
    public string DisplayName => $"{IcaoId} — {City} ({Name})";
}
