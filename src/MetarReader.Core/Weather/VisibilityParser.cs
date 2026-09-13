using System.Globalization;

namespace MetarReader.Core.Weather;

/// <summary>
/// Parses the aviationweather.gov "visib" field, which shows up as a plain
/// number ("6"), a "plus" value ("10+" meaning 10 statute miles or greater),
/// a simple fraction ("1/2"), or a mixed number ("1 1/4").
/// </summary>
public static class VisibilityParser
{
    public static string Parse(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return "unknown";
        }

        var text = raw.Trim();

        var orMore = text.EndsWith('+');
        if (orMore)
        {
            text = text[..^1];
        }

        if (!TryParseMiles(text, out var miles))
        {
            return "unknown";
        }

        var unit = miles <= 1 ? "mile" : "miles";
        return orMore ? $"{FormatMiles(text)} {unit} or more" : $"{FormatMiles(text)} {unit}";
    }

    private static bool TryParseMiles(string text, out double miles)
    {
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        miles = 0;

        foreach (var part in parts)
        {
            if (part.Contains('/'))
            {
                var fractionParts = part.Split('/');
                if (fractionParts.Length != 2 ||
                    !double.TryParse(fractionParts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var numerator) ||
                    !double.TryParse(fractionParts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var denominator) ||
                    denominator == 0)
                {
                    return false;
                }

                miles += numerator / denominator;
            }
            else if (double.TryParse(part, NumberStyles.Number, CultureInfo.InvariantCulture, out var whole))
            {
                miles += whole;
            }
            else
            {
                return false;
            }
        }

        return parts.Length > 0;
    }

    private static string FormatMiles(string originalText) => originalText.Trim();
}
