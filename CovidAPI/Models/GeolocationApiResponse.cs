using System.Collections.Generic;

/// <summary>
/// Represents the response from a geolocation API.
/// </summary>
public class GeolocationApiResponse
{
    /// <summary>
    /// Gets or sets the list of geolocation results.
    /// </summary>
    public List<GeolocationResult> Results { get; set; }
}

/// <summary>
/// Represents a geolocation result.
/// </summary>
public class GeolocationResult
{
    /// <summary>
    /// Gets or sets the components of the geolocation.
    /// </summary>
    public GeolocationComponents Components { get; set; }

    /// <summary>
    /// Gets or sets the geometry information of the geolocation.
    /// </summary>
    public Geometry Geometry { get; set; }
}

/// <summary>
/// Represents the components of a geolocation.
/// </summary>
public class GeolocationComponents
{
    /// <summary>
    /// Gets or sets the country of the geolocation.
    /// </summary>
    public string Country { get; set; }
}

/// <summary>
/// Represents the geometry information of a geolocation.
/// </summary>
public class Geometry
{
    /// <summary>
    /// Gets or sets the latitude of the geolocation.
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Gets or sets the longitude of the geolocation.
    /// </summary>
    public double Lng { get; set; }
}
