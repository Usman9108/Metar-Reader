using MetarReader.Core.Models;

namespace MetarReader.Core.Clients;

public interface IAviationWeatherClient
{
    /// <summary>
    /// Fetches the most recent METAR observation for the given ICAO station id
    /// (e.g. "KHIO"), or null if the station is unknown or has no current observation.
    /// </summary>
    Task<MetarObservation?> GetLatestMetarAsync(string icaoId, CancellationToken cancellationToken = default);
}
