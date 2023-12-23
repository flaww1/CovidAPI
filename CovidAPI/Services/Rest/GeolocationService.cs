using CovidAPI.Models;
using CovidAPI.Services.Rest;
using Newtonsoft.Json;

public class GeolocationCache
{
    private readonly Dictionary<string, GeolocationApiResponse> _cache = new Dictionary<string, GeolocationApiResponse>();

    public bool TryGetFromCache(string country, out GeolocationApiResponse geolocationResponse)
    {
        geolocationResponse = null;
        if (_cache.TryGetValue(country, out var cachedResponse))
        {
            geolocationResponse = cachedResponse;
            return true;
        }
        return false;
    }

    public void AddToCache(string country, GeolocationApiResponse geolocationResponse)
    {
        if (!_cache.ContainsKey(country))
        {
            _cache.Add(country, geolocationResponse);
        }
    }
}

public class GeolocationService : IGeolocationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;
    private readonly ILogger<GeolocationService> _logger;
    private readonly GeolocationCache _geolocationCache;

    public GeolocationService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GeolocationService> logger, GeolocationCache geolocationCache)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["OpenCage:ApiKey"];
        _logger = logger;
        _geolocationCache = geolocationCache;

        // Validate API key
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogError("Geolocation API key is missing or invalid.");
            // Throw an exception or handle the case accordingly.
        }
    }

    public async Task<GeolocationApiResponse> GetGeolocationInfoAsync(string country)
    {
        // Check if the information is already in the cache
        if (_geolocationCache.TryGetFromCache(country, out var cachedResponse))
        {
            _logger.LogInformation($"Geolocation info for {country} found in cache.");
            return cachedResponse;
        }

        try
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var requestUrl = $"https://api.opencagedata.com/geocode/v1/json?q={country}&key={_apiKey}";
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
                            foreach (var result in geolocationResponse.Results)
                            {
                                var geometry = result.Geometry;

                                if (geometry != null)
                                {
                                    var geolocation = new GeolocationComponents
                                    {
                                        Country = result.Components?.Country
                                    };

                                    // Access latitude and longitude via geometry object
                                    var latitude = geometry.Lat;
                                    var longitude = geometry.Lng;

                                    // Add the response to the cache
                                    _geolocationCache.AddToCache(country, geolocationResponse);

                                    _logger.LogInformation($"Geolocation Info: {JsonConvert.SerializeObject(geolocation)}");

                                    return geolocationResponse;
                                }
                            }
                            _logger.LogWarning("Geolocation API response does not contain geometry information.");
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


