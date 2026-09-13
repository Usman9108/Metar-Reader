using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetarReader.Core.Clients;

/// <summary>
/// Raw shape of one element returned by aviationweather.gov's
/// GET /api/data/metar?ids=...&amp;format=json endpoint. Only the fields the
/// app uses are declared. "wdir" and "visib" are typed as <see cref="JsonElement"/>
/// because the API returns them as either a number or a string (e.g. "VRB", "10+").
/// </summary>
internal sealed class AviationWeatherApiRecord
{
    /// <summary>Four-letter ICAO station identifier.</summary>
    [JsonPropertyName("icaoId")]
    public string? IcaoId { get; set; }

    /// <summary>Descriptive station name, e.g. "Portland-Hillsboro Airport, OR, US".</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Observation time as a Unix timestamp (seconds since epoch).</summary>
    [JsonPropertyName("obsTime")]
    public long? ObsTime { get; set; }

    /// <summary>Air temperature in degrees Celsius.</summary>
    [JsonPropertyName("temp")]
    public double? Temp { get; set; }

    /// <summary>Dewpoint in degrees Celsius.</summary>
    [JsonPropertyName("dewp")]
    public double? Dewp { get; set; }

    /// <summary>Wind direction: a number of degrees, or the string "VRB" for variable wind.</summary>
    [JsonPropertyName("wdir")]
    public JsonElement Wdir { get; set; }

    /// <summary>Sustained wind speed in knots.</summary>
    [JsonPropertyName("wspd")]
    public int? Wspd { get; set; }

    /// <summary>Gust speed in knots.</summary>
    [JsonPropertyName("wgst")]
    public int? Wgst { get; set; }

    /// <summary>Visibility: a number of statute miles, or a string such as "10+" or "1/2".</summary>
    [JsonPropertyName("visib")]
    public JsonElement Visib { get; set; }

    /// <summary>Altimeter setting in hectopascals.</summary>
    [JsonPropertyName("altim")]
    public double? Altim { get; set; }

    /// <summary>Present-weather codes, e.g. "-RA BR".</summary>
    [JsonPropertyName("wxString")]
    public string? WxString { get; set; }

    /// <summary>Reported sky condition layers.</summary>
    [JsonPropertyName("clouds")]
    public List<AviationWeatherCloudDto>? Clouds { get; set; }

    /// <summary>The original, unmodified METAR text.</summary>
    [JsonPropertyName("rawOb")]
    public string? RawOb { get; set; }
}

/// <summary>Raw shape of one entry in a <see cref="AviationWeatherApiRecord.Clouds"/> list.</summary>
internal sealed class AviationWeatherCloudDto
{
    /// <summary>Coverage code: <c>FEW</c>, <c>SCT</c>, <c>BKN</c>, <c>OVC</c>, or <c>VV</c>.</summary>
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    /// <summary>Height of the layer's base above ground, in feet.</summary>
    [JsonPropertyName("base")]
    public int? Base { get; set; }
}
