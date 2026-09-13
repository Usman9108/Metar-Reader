using MetarReader.Core.Clients;
using MetarReader.Core.Data;
using MetarReader.Core.Models;
using MetarReader.Core.Weather;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MetarReader.Web.Pages;

public class IndexModel(IAviationWeatherClient weatherClient, ILogger<IndexModel> logger) : PageModel
{
    public IReadOnlyList<Airport> Airports { get; private set; } = [];

    public void OnGet()
    {
        Airports = AirportCatalog.All;
    }

    /// <summary>AJAX endpoint backing the airport picker: GET ?handler=Weather&amp;icao=KHIO</summary>
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
                stationName = report.StationName,
                summary = report.Summary,
                details = report.Details,
                rawMetar = report.RawMetar,
                observedAtUtc = report.ObservedAtUtc.ToString("u"),
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
