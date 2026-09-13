namespace MetarReader.Core.Weather;

/// <summary>
/// Translates the METAR "present weather" string (e.g. "-RA", "+TSRA", "BR")
/// into plain-English phrases. Covers the combinations common in US METARs;
/// falls back to a generic code-by-code join for anything more exotic.
/// </summary>
public static class WeatherPhenomenaDecoder
{
    private static readonly Dictionary<string, string> FullCodePhrases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["TSRA"] = "thunderstorms with rain",
        ["TSSN"] = "thunderstorms with snow",
        ["TSGR"] = "thunderstorms with hail",
        ["SHRA"] = "rain showers",
        ["SHSN"] = "snow showers",
        ["SHGR"] = "hail showers",
        ["SHGS"] = "small hail showers",
        ["FZRA"] = "freezing rain",
        ["FZDZ"] = "freezing drizzle",
        ["FZFG"] = "freezing fog",
        ["MIFG"] = "shallow fog",
        ["BCFG"] = "patchy fog",
        ["PRFG"] = "fog banks",
        ["DRSN"] = "drifting snow",
        ["BLSN"] = "blowing snow",
        ["BLDU"] = "blowing dust",
        ["BLSA"] = "blowing sand",
        ["DRDU"] = "drifting dust",
        ["DRSA"] = "drifting sand",
        ["TS"] = "a thunderstorm",
        ["SH"] = "rain showers",
    };

    private static readonly Dictionary<string, string> PhenomenaPhrases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["DZ"] = "drizzle",
        ["RA"] = "rain",
        ["SN"] = "snow",
        ["SG"] = "snow grains",
        ["IC"] = "ice crystals",
        ["PL"] = "ice pellets",
        ["GR"] = "hail",
        ["GS"] = "small hail",
        ["UP"] = "unknown precipitation",
        ["BR"] = "mist",
        ["FG"] = "fog",
        ["FU"] = "smoke",
        ["VA"] = "volcanic ash",
        ["DU"] = "dust",
        ["SA"] = "sand",
        ["HZ"] = "haze",
        ["PY"] = "spray",
        ["PO"] = "dust whirls",
        ["SQ"] = "squalls",
        ["FC"] = "a funnel cloud",
        ["SS"] = "a sandstorm",
        ["DS"] = "a duststorm",
    };

    public static IReadOnlyList<string> Decode(string? wxString)
    {
        if (string.IsNullOrWhiteSpace(wxString))
        {
            return [];
        }

        var tokens = wxString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var phrases = new List<string>();

        foreach (var token in tokens)
        {
            var phrase = DecodeToken(token);
            if (phrase is not null)
            {
                phrases.Add(phrase);
            }
        }

        return phrases;
    }

    private static string? DecodeToken(string token)
    {
        if (token.Equals("NSW", StringComparison.OrdinalIgnoreCase))
        {
            return "no significant weather";
        }

        var remaining = token;
        string? intensity = null;

        if (remaining.StartsWith('-'))
        {
            intensity = "light";
            remaining = remaining[1..];
        }
        else if (remaining.StartsWith('+'))
        {
            intensity = "heavy";
            remaining = remaining[1..];
        }
        else if (remaining.StartsWith("VC", StringComparison.OrdinalIgnoreCase))
        {
            intensity = "nearby";
            remaining = remaining[2..];
        }

        if (remaining.Length == 0)
        {
            return intensity;
        }

        string body;
        if (FullCodePhrases.TryGetValue(remaining, out var exactPhrase))
        {
            body = exactPhrase;
        }
        else
        {
            var parts = DecomposeIntoKnownCodes(remaining);
            if (parts.Count == 0)
            {
                return null;
            }

            body = string.Join(" and ", parts);
        }

        return intensity is null ? body : $"{intensity} {body}";
    }

    private static List<string> DecomposeIntoKnownCodes(string remaining)
    {
        var parts = new List<string>();

        while (remaining.Length >= 2)
        {
            var code = remaining[..2];
            if (FullCodePhrases.TryGetValue(code, out var descriptorPhrase))
            {
                parts.Add(descriptorPhrase);
            }
            else if (PhenomenaPhrases.TryGetValue(code, out var phenomenonPhrase))
            {
                parts.Add(phenomenonPhrase);
            }
            else
            {
                break;
            }

            remaining = remaining[2..];
        }

        return parts;
    }
}
