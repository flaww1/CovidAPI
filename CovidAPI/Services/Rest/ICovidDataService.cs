using System.Collections.Generic;
using System.Threading.Tasks;
using CovidAPI.Models;

namespace CovidAPI.Services.Rest
{
    public interface ICovidDataService
    {
        Task<IEnumerable<CovidDataDTO>> GetAllDataAsync();
        Task<CovidDataDTO> GetDataByIdAsync(int id);
        Task<IEnumerable<CovidDataDTO>> GetDataByCountryAsync(string country);
        Task<IEnumerable<CovidDataDTO>> GetDataByYearAsync(int year);
        Task<IEnumerable<CovidDataDTO>> GetDataByWeekAsync(string week);
        Task AddDataAsync(CovidDataDTO covidDataDTO);
        Task UpdateDataAsync(CovidDataDTO covidDataDTO);
        Task DeleteDataAsync(int id);
        Task<double> CalculatePositivityRateAsync(int year, string week);
        Task<IEnumerable<CovidDataDTO>> GetDataByCountryAsync(string country, bool includeGeolocation);
    }
}
