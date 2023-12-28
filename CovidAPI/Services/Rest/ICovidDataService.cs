using System.Collections.Generic;
using System.Threading.Tasks;
using CovidAPI.Models;

namespace CovidAPI.Services.Rest
{
    /// <summary>
    /// Defines methods for retrieving and managing COVID-19 data.
    /// </summary>
    public interface ICovidDataService
    {
        /// <summary>
        /// Gets all COVID-19 data.
        /// </summary>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data.</returns>
        Task<IEnumerable<CovidDataDTO>> GetAllDataAsync(bool includeGeolocation);

        /// <summary>
        /// Gets COVID-19 data by identifier.
        /// </summary>
        /// <param name="id">The identifier of the COVID-19 data.</param>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the COVID-19 data with the specified identifier.</returns>
        Task<CovidDataDTO> GetDataByIdAsync(int id, bool includeGeolocation);

        /// <summary>
        /// Gets COVID-19 data for a specific year.
        /// </summary>
        /// <param name="year">The year for which to retrieve COVID-19 data.</param>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data for the specified year.</returns>
        Task<IEnumerable<CovidDataDTO>> GetDataByYearAsync(int year, bool includeGeolocation);

        /// <summary>
        /// Gets COVID-19 data for a specific week.
        /// </summary>
        /// <param name="week">The week for which to retrieve COVID-19 data.</param>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data for the specified week.</returns>
        Task<IEnumerable<CovidDataDTO>> GetDataByWeekAsync(string week, bool includeGeolocation);
        /// <summary>
        /// Gets the total COVID-19 cases.
        /// </summary>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data representing the total cases.</returns>
        Task<IEnumerable<CovidDataDTO>> GetTotalCasesAsync(bool includeGeolocation);

        /// <summary>
        /// Gets COVID-19 data for new cases.
        /// </summary>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data representing new cases.</returns>
        Task<IEnumerable<CovidDataDTO>> GetNewCasesAsync(bool includeGeolocation);

        /// <summary>
        /// Gets the total COVID-19 tests.
        /// </summary>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data representing the total tests.</returns>
        Task<IEnumerable<CovidDataDTO>> GetTotalTestsAsync(bool includeGeolocation);
        /// <summary>
        /// Gets the COVID-19 testing rate.
        /// </summary>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data representing the testing rate.</returns>
        Task<IEnumerable<CovidDataDTO>> GetTestingRateAsync(bool includeGeolocation);

        /// <summary>
        /// Gets the COVID-19 positivity rate.
        /// </summary>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data representing the positivity rate.</returns>
        Task<IEnumerable<CovidDataDTO>> GetPositivityRateAsync(bool includeGeolocation);

        /// <summary>
        /// Gets COVID-19 geolocation data.
        /// </summary>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data with geolocation information.</returns>
        Task<IEnumerable<CovidDataDTO>> GetGeolocationAsync(bool includeGeolocation);
        /// <summary>
        /// Gets COVID-19 testing source data.
        /// </summary>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data with testing source information.</returns>
        Task<IEnumerable<CovidDataDTO>> GetTestingSourceAsync(bool includeGeolocation);

        /// <summary>
        /// Gets COVID-19 data for specified countries and includes geolocation information if requested.
        /// </summary>
        /// <param name="countries">A list of country names to retrieve data for.</param>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data for the specified countries.</returns>
        Task<IEnumerable<CovidDataDTO>> GetComparisonsAsync(List<string> countries, bool includeGeolocation);

        /// <summary>
        /// Gets COVID-19 population data.
        /// </summary>
        /// <param name="includeGeolocation">Indicates whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of COVID-19 data with population information.</returns>
        Task<IEnumerable<CovidDataDTO>> GetPopulationDataAsync(bool includeGeolocation);

        /// <summary>
        /// Gets a collection of all distinct weeks for COVID-19 data.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of strings representing distinct weeks.</returns>
        Task<IEnumerable<string>> GetAllWeeksAsync();

        /// <summary>
        /// Gets information about COVID-19 testing rates.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a string with information about testing rates.</returns>
        Task<string> GetTestingRateInfoAsync();

        /// <summary>
        /// Gets a collection of all distinct countries for COVID-19 data.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of strings representing distinct countries.</returns>
        Task<IEnumerable<string>> GetAllCountriesAsync();

        /// <summary>
        /// Checks whether COVID-19 data exists for a specific country and week.
        /// </summary>
        /// <param name="country">The name of the country.</param>
        /// <param name="week">The week for which data existence is checked.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating whether data exists.</returns>
        Task<bool> DataExistsForCountryAndWeekAsync(string country, string week);

        /// <summary>
        /// Gets COVID-19 data for a specific country and week.
        /// </summary>
        /// <param name="country">The name of the country.</param>
        /// <param name="week">The week for which data is retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CovidData"/> object.</returns>
        Task<CovidData> GetDataByCountryAndWeekAsync(string country, string week);

        /// <summary>
        /// Adds new COVID-19 data using the provided data transfer object (DTO).
        /// </summary>
        /// <param name="covidDataDTO">The data transfer object containing COVID-19 data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddDataAsync(CovidDataDTO covidDataDTO);

        /// <summary>
        /// Updates existing COVID-19 data using the provided data transfer object (DTO).
        /// </summary>
        /// <param name="covidDataDTO">The data transfer object containing updated COVID-19 data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateDataAsync(CovidDataDTO covidDataDTO);

        /// <summary>
        /// Deletes COVID-19 data with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the COVID-19 data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteDataAsync(int id);

        /// <summary>
        /// Gets COVID-19 data for a specific country with an option to include geolocation information.
        /// </summary>
        /// <param name="country">The name of the country.</param>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a collection of <see cref="CovidDataDTO"/> objects.</returns>
        Task<IEnumerable<CovidDataDTO>> GetDataByCountryAsync(string country, bool includeGeolocation);

    }
}
