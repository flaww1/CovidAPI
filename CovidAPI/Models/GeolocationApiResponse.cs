namespace CovidAPI.Models
{
    public class GeolocationApiResponse
    {
        public List<GeolocationResult> Results { get; set; }
    }

    public class GeolocationResult
    {
        public GeolocationComponents Components { get; set; }
    }

    public class GeolocationComponents
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
    }


}
