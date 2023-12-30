/// <summary>
/// Represents a service for retrieving geolocation information related to COVID-19 data.
/// </summary>
public interface IGeolocationService
{
    /// <summary>
    /// Gets geolocation information asynchronously for the specified country.
    /// </summary>
    /// <param name="country">The name of the country for which geolocation information is requested.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a <see cref="GeolocationApiResponse"/> object.</returns>
    Task<GeolocationApiResponse> GetGeolocationInfoAsync(string country);
}
