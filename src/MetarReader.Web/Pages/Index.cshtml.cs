using MetarReader.Core.Clients;
using MetarReader.Core.Data;
using MetarReader.Core.Models;
using MetarReader.Core.Weather;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MetarReader.Web.Pages;

/// <summary>Backs the Metar Reader home page: the airport picker and its weather-briefing AJAX endpoint.</summary>
/// <param name="weatherClient">Fetches current METAR observations from aviationweather.gov.</param>
/// <param name="logger">Logs failures reaching the upstream weather service.</param>
public class IndexModel(IAviationWeatherClient weatherClient, ILogger<IndexModel> logger) : PageModel
{
    /// <summary>The airports offered in the picker.</summary>
    public IReadOnlyList<Airport> Airports { get; private set; } = [];

    /// <summary>Loads the airport catalog for the picker.</summary>
    public void OnGet()
    {
        Airports = AirportCatalog.All;
    }

    /// <summary>AJAX endpoint backing the airport picker: GET ?handler=Weather&amp;icao=KHIO</summary>
    /// <param name="icao">The selected airport's four-letter ICAO identifier.</param>
    /// <param name="cancellationToken">A token that cancels the request if the client disconnects.</param>
    /// <returns>
    /// A JSON payload with <c>success: true</c> and the decoded weather report, or
    /// <c>success: false</c> and a user-facing <c>error</c> message.
    /// </returns>
    public async Task<JsonResult> OnGetWeatherAsync(string icao, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(icao))
        {
            return new JsonResult(new { success = false, error = "Please choose an airport." });
        }

        try
        {
            var observation = await weatherClient.GetLatestMetarAsync(icao.Trim(), cancellationToken);
            if (observation is null)
            {
                return new JsonResult(new
                {
                    success = false,
                    error = $"No current weather observation is available for {icao.ToUpperInvariant()} right now.",
                });
            }

            var report = MetarDecoder.Decode(observation);

            return new JsonResult(new
            {
                success = true,
                icaoId = observation.IcaoId,
                stationName = report.StationName,
                summary = report.Summary,
                skyPhrase = report.SkyPhrase,
                details = report.Details,
                rawMetar = report.RawMetar,
                observedAtUtc = report.ObservedAtUtc.ToString("u"),
                flightCategory = report.FlightCategory.ToString().ToUpperInvariant(),
                tempF = report.TempF,
                windDirectionDegrees = report.WindDirectionDegrees,
                isVariableWind = report.IsVariableWind,
                windSpeedMph = report.WindSpeedMph,
                windGustMph = report.WindGustMph,
                visibilityMiles = report.VisibilityMiles,
                visibilityPhrase = report.VisibilityPhrase,
                ceilingFeet = report.CeilingFeet,
                altimeterInHg = report.AltimeterInHg,
            });
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            logger.LogWarning(ex, "Failed to reach aviationweather.gov for {Icao}", icao);
            return new JsonResult(new
            {
                success = false,
                error = "Couldn't reach the weather service right now — please try again in a moment.",
            });
        }
    }
}
