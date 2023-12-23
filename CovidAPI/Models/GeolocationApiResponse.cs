public class GeolocationApiResponse
{
    public List<GeolocationResult> Results { get; set; }
}

public class GeolocationResult
{
    public GeolocationComponents Components { get; set; }
    public Geometry Geometry { get; set; }
}

public class GeolocationComponents
{
    public string Country { get; set; }
}

public class Geometry
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}
