using System.Text.Json;
using MetarReader.Core.Models;

namespace MetarReader.Core.Clients;

/// <summary>
/// Talks to aviationweather.gov's data API
/// (https://aviationweather.gov/api/data/metar?ids=...&amp;format=json).
/// </summary>
/// <param name="httpClient">
/// The <see cref="HttpClient"/> used to call the API. Its <see cref="HttpClient.BaseAddress"/>
/// should be set to aviationweather.gov's base URL by the caller (typically via
/// <c>IHttpClientFactory</c> registration).
/// </param>
public sealed class AviationWeatherClient(HttpClient httpClient) : IAviationWeatherClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    /// <exception cref="HttpRequestException">The HTTP request failed or returned a non-success status code.</exception>
    public async Task<MetarObservation?> GetLatestMetarAsync(string icaoId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(icaoId);

        var url = $"api/data/metar?ids={Uri.EscapeDataString(icaoId)}&format=json";
        using var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(body))
        {
            // Unknown/invalid station ids return an empty body rather than "[]".
            return null;
        }

        var records = JsonSerializer.Deserialize<List<AviationWeatherApiRecord>>(body, JsonOptions);

        if (records is null || records.Count == 0)
        {
            return null;
        }

        var latest = records.OrderByDescending(r => r.ObsTime ?? 0).First();
        return Map(latest);
    }

    /// <summary>Converts a raw API record into the app's normalized <see cref="MetarObservation"/> model.</summary>
    private static MetarObservation Map(AviationWeatherApiRecord record)
    {
        var (windDirectionDegrees, isVariableWind) = ParseWindDirection(record.Wdir);
        var visibilityRaw = ParseVisibility(record.Visib);

        var clouds = (record.Clouds ?? [])
            .Where(c => !string.IsNullOrWhiteSpace(c.Cover))
            .Select(c => new CloudLayer(c.Cover!, c.Base))
            .ToList();

        var observedAt = record.ObsTime is { } obsTime
            ? DateTimeOffset.FromUnixTimeSeconds(obsTime)
            : DateTimeOffset.UtcNow;

        return new MetarObservation(
            record.IcaoId ?? "UNKNOWN",
            record.Name,
            observedAt,
            record.Temp,
            record.Dewp,
            windDirectionDegrees,
            isVariableWind,
            record.Wspd,
            record.Wgst,
            visibilityRaw,
            clouds,
            record.WxString,
            record.Altim,
            record.RawOb ?? string.Empty);
    }

    /// <summary>
    /// Reads the API's "wdir" field, which is a plain number of degrees, or the
    /// string "VRB" when the reported wind direction is variable.
    /// </summary>
    private static (int? Degrees, bool IsVariable) ParseWindDirection(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Number when element.TryGetInt32(out var degrees):
                return (degrees, false);
            case JsonValueKind.String:
                var text = element.GetString();
                if (string.Equals(text, "VRB", StringComparison.OrdinalIgnoreCase))
                {
                    return (null, true);
                }

                return int.TryParse(text, out var parsed) ? (parsed, false) : (null, false);
            default:
                return (null, false);
        }
    }

    /// <summary>Reads the API's "visib" field, which is either a number or a string (e.g. "10+", "1/2").</summary>
    private static string? ParseVisibility(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Number => element.GetRawText(),
        JsonValueKind.String => element.GetString(),
        _ => null,
    };
}
