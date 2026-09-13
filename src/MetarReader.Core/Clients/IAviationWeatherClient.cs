using MetarReader.Core.Models;

namespace MetarReader.Core.Clients;

/// <summary>Fetches current METAR observations from an external weather data source.</summary>
public interface IAviationWeatherClient
{
    /// <summary>
    /// Fetches the most recent METAR observation for the given ICAO station id
    /// (e.g. "KHIO"), or null if the station is unknown or has no current observation.
    /// </summary>
    /// <param name="icaoId">The station's four-letter ICAO identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The latest <see cref="MetarObservation"/> for the station, or null if none is available.</returns>
    Task<MetarObservation?> GetLatestMetarAsync(string icaoId, CancellationToken cancellationToken = default);
}
