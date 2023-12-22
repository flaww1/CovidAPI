using System;
using System.Net.Http;
using System.Threading.Tasks;
using CovidAPI.Models;
using CovidAPI.Services.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class GeolocationService : IGeolocationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;
    private readonly ILogger<GeolocationService> _logger;

    public GeolocationService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GeolocationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["OpenCage:ApiKey"];
        _logger = logger;

        // Validate API key
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogError("Geolocation API key is missing or invalid.");
            // Throw an exception or handle the case accordingly.
        }
    }

    public async Task<GeolocationApiResponse> GetGeolocationInfoAsync(string country)
    {
        try
        {
            // Create an HttpClient instance using the factory
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                // Make an API request to OpenCage Geocoding API
                var response = await httpClient.GetAsync($"https://api.opencagedata.com/geocode/v1/json?q={country}&key={_apiKey}");

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response content into GeolocationApiResponse
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<GeolocationApiResponse>(content);
                }
                else
                {
                    // Log the error and return a meaningful response
                    _logger.LogError($"Geolocation API request failed with status code {response.StatusCode}");
                    return new GeolocationApiResponse { /* Add error information */ };
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception and return a meaningful response
            _logger.LogError($"An error occurred while processing the Geolocation API request: {ex.Message}");
            return new GeolocationApiResponse { /* Add error information */ };
        }
    }
}
