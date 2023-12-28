using CovidAPI.Models;
using CovidAPI.Services.Rest;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Net;
using System.Text.Json;

/// <summary>
/// Represents a cache for storing geolocation data to avoid redundant API calls.
/// </summary>
public class GeolocationCache
{
    private Dictionary<string, GeolocationApiResponse> _cache { get; set; }
    private readonly object _lockObject = new object();
    private const string CacheFilePath = "cache.json";

    public GeolocationCache()
    {
        _cache = new Dictionary<string, GeolocationApiResponse>();
        LoadCacheFromFile();
    }
    /// <summary>
    /// Tries to retrieve geolocation data from the cache for a specific country.
    /// </summary>
    /// <param name="country">The country for which to retrieve geolocation data.</param>
    /// <param name="geolocationResponse">The retrieved geolocation response if successful.</param>
    /// <returns>True if the data is successfully retrieved from the cache; otherwise, false.</returns>
    public bool TryGetFromCache(string country, out GeolocationApiResponse geolocationResponse)
    {
        lock (_lockObject)
        {
            geolocationResponse = null;
            if (_cache.TryGetValue(country, out var cachedResponse))
            {
                geolocationResponse = cachedResponse;
                Console.WriteLine($"Geolocation data retrieved from cache for country: {country}");
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// Adds geolocation data to the cache for a specific country.
    /// </summary>
    /// <param name="country">The country for which to add geolocation data to the cache.</param>
    /// <param name="geolocationResponse">The geolocation response to be added to the cache.</param>
    public void AddToCache(string country, GeolocationApiResponse geolocationResponse)
    {
        lock (_lockObject)
        {
            if (!_cache.ContainsKey(country))
            {
                _cache.Add(country, geolocationResponse);
                Console.WriteLine($"Geolocation data added to cache for country: {country}");
                SaveCacheToFile();
            }
        }
    }
    /// <summary>
    /// Loads geolocation data from a file into the cache if the file exists.
    /// </summary>
    private void LoadCacheFromFile()
    {
        if (File.Exists(CacheFilePath))
        {
            var json = File.ReadAllText(CacheFilePath);
            _cache = JsonConvert.DeserializeObject<Dictionary<string, GeolocationApiResponse>>(json);
        }
    }
    /// <summary>
    /// Saves the current geolocation cache to a file in JSON format.
    /// </summary>
    private void SaveCacheToFile()
    {
        var json = JsonConvert.SerializeObject(_cache);
        File.WriteAllText(CacheFilePath, json);
    }
}

/// <summary>
/// Represents a service for retrieving geolocation information for a specific country.
/// </summary>
public class GeolocationService : IGeolocationService
{

    private readonly IDistributedCache _distributedCache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;
    private readonly ILogger<GeolocationService> _logger;
    private readonly GeolocationCache _geolocationCache;

    public GeolocationService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GeolocationService> logger, GeolocationCache geolocationCache, IDistributedCache distributedCache)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["OpenCage:ApiKey"];
        _logger = logger;
        _geolocationCache = geolocationCache;
        _distributedCache = distributedCache;

        // Validate API key
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogError("Geolocation API key is missing or invalid.");
            // Throw an exception or handle the case accordingly.
        }
    
    }
    /// <summary>
    /// Retrieves geolocation information asynchronously for a specific country.
    /// </summary>
    /// <param name="country">The country for which to retrieve geolocation information.</param>
    /// <returns>An asynchronous task that returns the geolocation response.</returns>
    public async Task<GeolocationApiResponse> GetGeolocationInfoAsync(string country)
    {
        try
        {
            // Check if the information is in the cache
            if (_geolocationCache.TryGetFromCache(country, out var cachedResponse))
            {
                _logger.LogInformation($"Geolocation info retrieved from cache for {country}");
                return cachedResponse;
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var encodedCountry = Uri.EscapeDataString(country);
                var requestUrl = $"https://api.opencagedata.com/geocode/v1/json?q={encodedCountry}&key={_apiKey}";

                _logger.LogInformation($"Sending request to Geolocation API: {requestUrl}");

                var response = await httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Geolocation API Response: {content}");

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var geolocationResponse = JsonConvert.DeserializeObject<GeolocationApiResponse>(content);

                        if (geolocationResponse != null && geolocationResponse.Results.Any())
                        {
                            GeolocationResult geolocation = null;
                            foreach (var result in geolocationResponse.Results)
                            {
                                var components = result.Components;

                                if (components != null)
                                {
                                    geolocation = new GeolocationResult
                                    {
                                        Components = new GeolocationComponents
                                        {
                                            Country = components.Country,
                                        },
                                        Geometry = new Geometry
                                        {
                                            Lat = result.Geometry.Lat,
                                            Lng = result.Geometry.Lng
                                        }
                                    };

                                    break;
                                }
                            }

                            if (geolocation != null)
                            {
                                // Cache the result
                                _geolocationCache.AddToCache(country, geolocationResponse);

                                _logger.LogInformation($"Geolocation Info: {JsonConvert.SerializeObject(geolocation)}");

                                return geolocationResponse;
                            }
                            else
                            {
                                _logger.LogWarning("Geolocation API response does not contain geolocation information.");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Geolocation API response does not contain results.");
                        }
                    }
                    else
                    {
                        _logger.LogError("Geolocation API response content is empty.");
                    }
                }
                else
                {
                    _logger.LogError($"Geolocation API request failed with status code {response.StatusCode}");
                }

                // Return a meaningful response in case of errors
                return new GeolocationApiResponse { /* Add error information */ };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while processing the Geolocation API request: {ex.Message}");
            return new GeolocationApiResponse { /* Add error information */ };
        }
    }

}
