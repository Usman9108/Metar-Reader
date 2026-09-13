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
    [JsonPropertyName("icaoId")]
    public string? IcaoId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("obsTime")]
    public long? ObsTime { get; set; }

    [JsonPropertyName("temp")]
    public double? Temp { get; set; }

    [JsonPropertyName("dewp")]
    public double? Dewp { get; set; }

    [JsonPropertyName("wdir")]
    public JsonElement Wdir { get; set; }

    [JsonPropertyName("wspd")]
    public int? Wspd { get; set; }

    [JsonPropertyName("wgst")]
    public int? Wgst { get; set; }

    [JsonPropertyName("visib")]
    public JsonElement Visib { get; set; }

    [JsonPropertyName("altim")]
    public double? Altim { get; set; }

    [JsonPropertyName("wxString")]
    public string? WxString { get; set; }

    [JsonPropertyName("clouds")]
    public List<AviationWeatherCloudDto>? Clouds { get; set; }

    [JsonPropertyName("rawOb")]
    public string? RawOb { get; set; }
}

internal sealed class AviationWeatherCloudDto
{
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    [JsonPropertyName("base")]
    public int? Base { get; set; }
}
