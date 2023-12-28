using System.Collections.Generic;
using System.Threading.Tasks;
using CovidAPI.Models;

namespace CovidAPI.Services.Rest
{
    public interface ICovidDataService
    {
        Task<IEnumerable<CovidDataDTO>> GetAllDataAsync(bool includeGeolocation);
        Task<CovidDataDTO> GetDataByIdAsync(int id, bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetDataByYearAsync(int year, bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetDataByWeekAsync(string week, bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetTotalCasesAsync(bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetNewCasesAsync(bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetTotalTestsAsync(bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetTestingRateAsync(bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetPositivityRateAsync(bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetGeolocationAsync(bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetTestingSourceAsync(bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetComparisonsAsync(List<string> countries, bool includeGeolocation);
        Task<IEnumerable<CovidDataDTO>> GetPopulationDataAsync(bool includeGeolocation);
        Task<IEnumerable<string>> GetAllWeeksAsync();
        Task<string> GetTestingRateInfoAsync();
        Task<IEnumerable<string>> GetAllCountriesAsync();
        Task<bool> DataExistsForCountryAndWeekAsync(string country, string week);
        Task<CovidData> GetDataByCountryAndWeekAsync(string country, string week);
        Task AddDataAsync(CovidDataDTO covidDataDTO);
        Task UpdateDataAsync(CovidDataDTO covidDataDTO);
        Task DeleteDataAsync(int id);
        Task<IEnumerable<CovidDataDTO>> GetDataByCountryAsync(string country, bool includeGeolocation);
    }
}
