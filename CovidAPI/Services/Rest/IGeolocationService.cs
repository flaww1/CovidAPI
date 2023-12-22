using CovidAPI.Models;
namespace CovidAPI.Services.Rest
{
    public interface IGeolocationService
    {
        Task<GeolocationApiResponse> GetGeolocationInfoAsync(string country);
    }

}
