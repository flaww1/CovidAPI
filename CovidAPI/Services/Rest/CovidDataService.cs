using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CovidAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CovidAPI.Services.Rest
{

    /// <summary>
    /// Service responsible for handling COVID-19 data operations, including fetching, updating, and deleting data.
    /// </summary>
    public class CovidDataService : ICovidDataService
    {
        private readonly IGeolocationService _geolocationService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CovidDataService> _logger;
        private readonly GeolocationCache _geolocationCache;
        private readonly IOptions<DbContextOptions<ApplicationDbContext>> _dbContextOptions;

        public CovidDataService(ApplicationDbContext context, IGeolocationService geolocationService, ILogger<CovidDataService> logger, GeolocationCache geolocationCache, IOptions<DbContextOptions<ApplicationDbContext>> dbContextOptions)
        {
            _context = context;
            _geolocationService = geolocationService;
            _logger = logger;
            _geolocationCache = geolocationCache;
            _dbContextOptions = dbContextOptions;
        }

        /// <summary>
        /// Checks if data exists for a specific country and week.
        /// </summary>
        /// <param name="country">The country for which to check data.</param>
        /// <param name="week">The week for which to check data.</param>
        /// <returns>True if data exists; otherwise, false.</returns>       
        public async Task<bool> DataExistsForCountryAndWeekAsync(string country, string week)
        {
            // Assuming you have a method in your repository to check if data exists
            // You need to replace it with your actual implementation
            var existingData = await GetDataByCountryAndWeekAsync(country, week);

            // Check if data exists
            return existingData != null;
        }


        // <summary>
        /// Retrieves COVID-19 data for a specific country and week.
        /// </summary>
        /// <param name="country">The country for which to retrieve data.</param>
        /// <param name="week">The week for which to retrieve data.</param>
        /// <returns>The COVID-19 data for the specified country and week.</returns>
        public async Task<CovidData> GetDataByCountryAndWeekAsync(string country, string week)
        {
            // Assuming you have a DbContext named _context and a DbSet for CovidData
            // Replace this with your actual DbContext and DbSet
            return await _context.CovidData
                .FirstOrDefaultAsync(c => c.Country == country && c.Week == week);
        }

        /// <summary>
        /// Retrieves all COVID-19 data as DTOs, optionally including geolocation information.
        /// </summary>
        /// <param name="includeGeolocation">Specifies whether to include geolocation information.</param>
        /// <returns>A list of COVID-19 data DTOs.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetAllDataAsync(bool includeGeolocation)
        {
            try
            {
                var data = await _context.CovidData
                    .Select(d => MapToDTO(d))
                    .ToListAsync();

                if (includeGeolocation && data.Any())
                {
                    await FetchAndCacheGeolocationDataAsync(data);
                }

                return data;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, "An error occurred while fetching all data.");
                return Enumerable.Empty<CovidDataDTO>();
            }
        }
        /// <summary>
        /// Retrieves COVID-19 data for a specific ID.
        /// </summary>
        /// <param name="id">The ID of the COVID-19 data to retrieve.</param>
        /// <param name="includeGeolocation">Specifies whether to include geolocation information.</param>
        /// <returns>The COVID-19 data DTO for the specified ID.</returns>
        public async Task<CovidDataDTO> GetDataByIdAsync(int id, bool includeGeolocation)
        {
            var data = await _context.CovidData.FindAsync(id);
            var dataDTO = MapToDTO(data);

            if (includeGeolocation)
            {
                await UpdateGeolocationInDataAsync(new List<CovidDataDTO> { dataDTO });
            }

            return dataDTO;
        }
        /// <summary>
        /// Retrieves COVID-19 data for a specific year, optionally including geolocation information.
        /// </summary>
        /// <param name="year">The year for which to retrieve data.</param>
        /// <param name="includeGeolocation">Specifies whether to include geolocation information.</param>
        /// <returns>A list of COVID-19 data DTOs for the specified year.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetDataByYearAsync(int year, bool includeGeolocation)
        {
            var data = await _context.CovidData
                .Where(data => data.Year == year)
                .Select(data => MapToDTO(data))
                .ToListAsync();

            if (includeGeolocation)
            {
                await UpdateGeolocationInDataAsync(data);
            }

            return data;
        }

        /// <summary>
        /// Retrieves COVID-19 data for a specific week, optionally including geolocation information.
        /// </summary>
        /// <param name="week">The week for which to retrieve data.</param>
        /// <param name="includeGeolocation">Specifies whether to include geolocation information.</param>
        /// <returns>A list of COVID-19 data DTOs for the specified week.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetDataByWeekAsync(string week, bool includeGeolocation)
        {
            try
            {
                var data = await _context.CovidData
                    .Where(d => d.Week == week)
                    .Select(d => MapToDTO(d))
                    .ToListAsync();

                if (includeGeolocation && data.Any())
                {
                    await FetchAndCacheGeolocationDataAsync(data);
                }

                return data;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, $"An error occurred while fetching data for week {week}.");
                return null; // Or return an empty list based on your error-handling strategy
            }
        }





        /// <summary>
        /// Adds COVID-19 data to the database.
        /// </summary>
        /// <param name="covidDataDTO">The COVID-19 data DTO to add.</param>
        public async Task AddDataAsync(CovidDataDTO covidDataDTO)
        {
            _context.CovidData.Add(MapToEntity(covidDataDTO));
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates existing COVID-19 data in the database.
        /// </summary>
        /// <param name="covidDataDTO">The COVID-19 data DTO with updated information.</param>
        public async Task UpdateDataAsync(CovidDataDTO covidDataDTO)
        {
            var existingData = await _context.CovidData.FindAsync(covidDataDTO.Id);
            if (existingData == null)
            {
                throw new ArgumentException("Covid data not found");
            }

            MapToEntity(covidDataDTO, existingData);

            _context.CovidData.Update(existingData);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes COVID-19 data from the database by ID.
        /// </summary>
        /// <param name="id">The ID of the COVID-19 data to delete.</param>
        public async Task DeleteDataAsync(int id)
        {
            var covidData = await _context.CovidData.FindAsync(id);
            if (covidData != null)
            {
                _context.CovidData.Remove(covidData);
                await _context.SaveChangesAsync();
            }
        }



        /// <summary>
        /// Retrieves COVID-19 data for a specific country, optionally including geolocation information.
        /// </summary>
        /// <param name="country">The country for which to retrieve data.</param>
        /// <param name="includeGeolocation">Specifies whether to include geolocation information.</param>
        /// <returns>A list of COVID-19 data DTOs for the specified country.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetDataByCountryAsync(string country, bool includeGeolocation)
        {
            try
            {
                var data = await _context.CovidData
                    .Where(d => d.Country == country)
                    .Select(d => MapToDTO(d))
                    .ToListAsync();

                if (includeGeolocation && data.Any())
                {
                    await FetchAndCacheGeolocationDataAsync(data);
                }

                return data;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, $"An error occurred while fetching data for country {country}.");
                return null; // Or return an empty list based on your error-handling strategy
            }
        }
        /// <summary>
        /// Updates geolocation information in the provided collection of CovidDataDTO objects based on the country.
        /// </summary>
        /// <param name="data">The collection of CovidDataDTO objects.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateGeolocationInDataAsync(IEnumerable<CovidDataDTO> data)
        {
            var uniqueCountries = data.Select(d => d.Country).Distinct().ToList();

            foreach (var uniqueCountry in uniqueCountries)
            {
                // Check if the geolocation information is already in the cache
                if (_geolocationCache.TryGetFromCache(uniqueCountry, out var cachedResponse))
                {
                    // If cached, update geolocation in the data
                    await UpdateGeolocationAsync(data, uniqueCountry, cachedResponse);
                }
                else
                {
                    // If not cached, fetch geolocation information from the service
                    var geolocationResponse = await _geolocationService.GetGeolocationInfoAsync(uniqueCountry);

                    // Update geolocation in the data
                    await UpdateGeolocationAsync(data, uniqueCountry, geolocationResponse);

                    // Add the response to the cache
                    _geolocationCache.AddToCache(uniqueCountry, geolocationResponse);
                }
            }
        }

        /// <summary>
        /// Updates geolocation information in the provided CovidDataDTO objects based on the given GeolocationApiResponse.
        /// </summary>
        /// <param name="data">The collection of CovidDataDTO objects to be updated with geolocation information.</param>
        /// <param name="country">The country for which geolocation information is being updated.</param>
        /// <param name="geolocationResponse">The GeolocationApiResponse containing the geolocation information.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        private async Task UpdateGeolocationAsync(IEnumerable<CovidDataDTO> data, string country, GeolocationApiResponse geolocationResponse)
        {
            foreach (var covidData in data.Where(d => d.Country == country))
            {
                // Log the data before mapping
                _logger.LogInformation($"Before Mapping: {JsonConvert.SerializeObject(covidData)}");

                // Find the matching result in the geolocation response
                var result = geolocationResponse.Results.FirstOrDefault(r => r.Components?.Country == country);

                // Calculate total tests and cases for the specific year
                var yearData = data.Where(d => d.Country == country && d.Year == covidData.Year);
                covidData.TotalTestsYear = yearData.Sum(d => d.TestsDone);
                covidData.TotalCasesYear = yearData.Sum(d => d.NewCases);

                // Calculate per capita values, using a default population of 1 if it's null
                double population = covidData.Population; 
                covidData.PerCapitaTests = covidData.TotalTestsYear / population;
                covidData.PerCapitaCases = covidData.TotalCasesYear / population;

                // Check if a matching result was found
                if (result != null)
                {
                    // Assign the Geolocation and Geometry objects to the CovidDataDTO object
                    covidData.Geolocation = result.Components;
                    covidData.Geometry = result.Geometry;

                    // Log the data after mapping
                    _logger.LogInformation($"After Mapping: {JsonConvert.SerializeObject(covidData.Geolocation)}");
                }
                else
                {
                    _logger.LogWarning($"No geolocation data found for country {country}");
                }
            }
        }


        /// <summary>
        /// Maps a CovidDataDTO object to a CovidData entity. If an existingData object is provided, it will be updated; otherwise, a new CovidData entity will be created.
        /// </summary>
        /// <param name="dataDTO">The CovidDataDTO object to be mapped to a CovidData entity.</param>
        /// <param name="existingData">The existing CovidData entity to be updated. If null, a new entity will be created.</param>
        /// <returns>The mapped CovidData entity.</returns>
        private CovidData MapToEntity(CovidDataDTO dataDTO, CovidData existingData = null)
        {
            if (dataDTO == null)
            {
                return null;
            }

            if (existingData == null)
            {
                existingData = new CovidData();
            }

            existingData.Country = dataDTO.Country;
            existingData.CountryCode = dataDTO.CountryCode;
            existingData.Year = dataDTO.Year;
            existingData.Week = dataDTO.Week;
            existingData.Region = dataDTO.Region;
            existingData.RegionName = dataDTO.RegionName;
            existingData.NewCases = dataDTO.NewCases;
            existingData.TestsDone = dataDTO.TestsDone;
            existingData.Population = dataDTO.Population;
            existingData.PositivityRate = dataDTO.PositivityRate;
            existingData.TestingRate = dataDTO.TestingRate;
            existingData.TestingDataSource = dataDTO.TestingDataSource;

            return existingData;
        }

        /// <summary>
        /// Maps a CovidData entity to a CovidDataDTO object.
        /// </summary>
        /// <param name="data">The CovidData entity to be mapped to a CovidDataDTO object.</param>
        /// <returns>The mapped CovidDataDTO object.</returns>
        private static CovidDataDTO MapToDTO(CovidData data)
        {
            if (data == null)
            {
                return null;
            }

            return new CovidDataDTO
            {
                Id = data.Id,
                Country = data.Country,
                CountryCode = data.CountryCode,
                Year = data.Year,
                Week = data.Week,
                Region = data.Region,
                RegionName = data.RegionName,
                NewCases = data.NewCases,
                TestsDone = data.TestsDone,
                Population = data.Population,
                PositivityRate = data.PositivityRate,
                TestingRate = data.TestingRate,
                TestingDataSource = data.TestingDataSource,
                Geolocation = null // Default to null, as it's not provided in this method
            };
        }

        /// <summary>
        /// Retrieves a list of unique weeks available in the CovidData records.
        /// </summary>
        /// <returns>An asynchronous task that returns a collection of unique week values.</returns>
        public async Task<IEnumerable<string>> GetAllWeeksAsync()
        {
            try
            {
                var weeks = await _context.CovidData
                    .Select(data => data.Week)
                    .Distinct()
                    .ToListAsync();

                return weeks;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                throw new ApplicationException($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Fetches geolocation data for the unique countries in the provided CovidDataDTO collection,
        /// updates the geolocation information in the data, and caches the responses.
        /// </summary>
        /// <param name="data">The collection of CovidDataDTO objects for which to fetch geolocation data.</param>
        /// <returns>An asynchronous task representing the geolocation data fetching and updating process.</returns>
        private async Task FetchAndCacheGeolocationDataAsync(IEnumerable<CovidDataDTO> data)
        {
            // Fetch geolocation information for the unique countries
            var uniqueCountries = data.Select(d => d.Country).Distinct().ToList();

            foreach (var uniqueCountry in uniqueCountries)
            {
                if (_geolocationCache.TryGetFromCache(uniqueCountry, out var cachedResponse))
                {
                    await UpdateGeolocationAsync(data, uniqueCountry, cachedResponse);
                }
                else
                {
                    var geolocationResponse = await _geolocationService.GetGeolocationInfoAsync(uniqueCountry);
                    await UpdateGeolocationAsync(data, uniqueCountry, geolocationResponse);
                    _geolocationCache.AddToCache(uniqueCountry, geolocationResponse);
                }
            }
        }


        private void LogAndThrowException(Exception ex, string message)
        {
            _logger.LogError($"{message} Error: {ex.Message}");
            throw new ApplicationException(message);
        }

        /// <summary>
        /// Retrieves total cases data by grouping CovidData records by country and calculating the sum of new cases for each country.
        /// </summary>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation data in the results.</param>
        /// <returns>An asynchronous task that returns a collection of CovidDataDTO objects representing total cases data.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetTotalCasesAsync(bool includeGeolocation)
        {
            try
            {
                var totalCasesData = await _context.CovidData
                    .GroupBy(d => d.Country)
                    .Select(group => new CovidDataDTO
                    {
                        Country = group.Key,
                        TotalCasesYear = group.Sum(d => d.NewCases),
                        Geolocation = null // Default to null, as it's not provided in this method
                    })
                    .ToListAsync();

                if (includeGeolocation && totalCasesData.Any())
                {
                    await FetchAndCacheGeolocationDataAsync(totalCasesData);
                }

                return totalCasesData;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, "An error occurred while fetching total cases data.");
                return Enumerable.Empty<CovidDataDTO>();
            }
        }

        /// <summary>
        /// Retrieves new cases data by grouping CovidData records by country and week, calculating the sum of new cases for each country and week.
        /// </summary>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation data in the results.</param>
        /// <returns>An asynchronous task that returns a collection of CovidDataDTO objects representing new cases data.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetNewCasesAsync(bool includeGeolocation)
        {
            try
            {
                var newCasesData = await _context.CovidData
                    .GroupBy(d => new { d.Country, d.Week })
                    .Select(group => new CovidDataDTO
                    {
                        Country = group.Key.Country,
                        Week = group.Key.Week,
                        NewCases = group.Sum(d => d.NewCases),
                        Geolocation = null // Default to null, as it's not provided in this method
                    })
                    .ToListAsync();

                if (includeGeolocation && newCasesData.Any())
                {
                    await FetchAndCacheGeolocationDataAsync(newCasesData);
                }

                return newCasesData;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, "An error occurred while fetching new cases data.");
                return Enumerable.Empty<CovidDataDTO>();
            }
        }



        /// <summary>
        /// Retrieves population data, including the country and population, with an option to include geolocation information.
        /// </summary>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation data in the results.</param>
        /// <returns>An asynchronous task that returns a collection of CovidDataDTO objects representing population data.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetPopulationDataAsync(bool includeGeolocation)
        {
            try
            {
                var populationData = await _context.CovidData
                    .Select(d => new CovidDataDTO
                    {
                        Country = d.Country,
                        Population = d.Population,
                        Geolocation = null // Default to null, as it's not provided in this method
                    })
                    .ToListAsync();

                if (includeGeolocation && populationData.Any())
                {
                    await FetchAndCacheGeolocationDataAsync(populationData);
                }

                return populationData;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, "An error occurred while fetching population data.");
                return Enumerable.Empty<CovidDataDTO>();
            }
        }

        /// <summary>
        /// Retrieves comparison data for specified countries, with an option to include geolocation information.
        /// </summary>
        /// <param name="countries">A list of country names for which to fetch comparison data.</param>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation data in the results.</param>
        /// <returns>An asynchronous task that returns a collection of CovidDataDTO objects representing comparison data.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetComparisonsAsync(List<string> countries, bool includeGeolocation)
        {
            try
            {
                var comparisonData = await _context.CovidData
                    .Where(d => countries.Contains(d.Country))
                    .Select(d => MapToDTO(d))
                    .ToListAsync();

                if (includeGeolocation && comparisonData.Any())
                {
                    await FetchAndCacheGeolocationDataAsync(comparisonData);
                }

                return comparisonData;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, "An error occurred while fetching comparison data.");
                return Enumerable.Empty<CovidDataDTO>();
            }
        }

        /// <summary>
        /// Retrieves total tests data, including the country and total tests for the specific year, with an option to include geolocation information.
        /// </summary>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation data in the results.</param>
        /// <returns>An asynchronous task that returns a collection of CovidDataDTO objects representing total tests data.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetTotalTestsAsync(bool includeGeolocation)
        {
            try
            {
                var totalTestsData = await _context.CovidData
                    .GroupBy(d => d.Country)
                    .Select(group => new CovidDataDTO
                    {
                        Country = group.Key,
                        TotalTestsYear = group.Sum(d => d.TestsDone), // Assuming there's a property TotalTests in CovidDataDTO
                        Geolocation = null
                    })
                    .ToListAsync();

                if (includeGeolocation && totalTestsData.Any())
                {
                    await FetchAndCacheGeolocationDataAsync(totalTestsData);
                }

                return totalTestsData;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, "An error occurred while fetching total tests data.");
                return Enumerable.Empty<CovidDataDTO>();
            }
        }

        /// <summary>
        /// Retrieves geolocation data, including the country and geolocation information, with an option to include geolocation information.
        /// </summary>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation data in the results.</param>
        /// <returns>An asynchronous task that returns a collection of CovidDataDTO objects representing geolocation data.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetGeolocationAsync(bool includeGeolocation)
        {
            try
            {
                var geolocationData = await _context.CovidData
                    .Select(d => new CovidDataDTO
                    {
                        Country = d.Country,
                        Geolocation = null // Assuming there's a property Geolocation in CovidDataDTO
                    })
                    .ToListAsync();

                if (includeGeolocation && geolocationData.Any())
                {
                    await FetchAndCacheGeolocationDataAsync(geolocationData);
                }

                return geolocationData;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, "An error occurred while fetching geolocation data.");
                return Enumerable.Empty<CovidDataDTO>();
            }
        }

        /// <summary>
        /// Retrieves testing source data, including the country, testing data source, with an option to include geolocation information.
        /// </summary>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation data in the results.</param>
        /// <returns>An asynchronous task that returns a collection of CovidDataDTO objects representing testing source data.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetTestingSourceAsync(bool includeGeolocation)
        {
            try
            {
                var testingSourceData = await _context.CovidData
                    .Select(d => new CovidDataDTO
                    {
                        Country = d.Country,
                        TestingDataSource = d.TestingDataSource,
                        Geolocation = null
                    })
                    .ToListAsync();

                if (includeGeolocation && testingSourceData.Any())
                {
                    await FetchAndCacheGeolocationDataAsync(testingSourceData);
                }

                return testingSourceData;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, "An error occurred while fetching testing source data.");
                return Enumerable.Empty<CovidDataDTO>();
            }
        }

        /// <summary>
        /// Retrieves testing rate information.
        /// </summary>
        /// <returns>An asynchronous task that returns a string representing testing rate information.</returns>
        public async Task<string> GetTestingRateInfoAsync()
        {
            try
            {
                // not implemented
                var testingRateInfo = "This is the testing rate information.";
                return testingRateInfo;
            }
            catch (Exception ex)
            {
                LogAndThrowException(ex, "An error occurred while fetching testing rate information.");
                return null;
            }
        }

        /// <summary>
        /// Retrieves tests done data, including the country, week, tests done, population, with an option to include geolocation information.
        /// </summary>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation data in the results.</param>
        /// <returns>An asynchronous task that returns a collection of CovidDataDTO objects representing tests done data.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetTestsDoneAsync(bool includeGeolocation)
        {
            var testsDoneData = await _context.CovidData
                .GroupBy(d => new { d.Country, d.Week })
                .Select(group => new CovidDataDTO
                {
                    Country = group.Key.Country,
                    Week = group.Key.Week,
                    TestsDone = group.Sum(d => d.TestsDone),
                    Population = group.Max(d => d.Population),
                    Geolocation = null // Default to null, as it's not provided in this method
                })
                .ToListAsync();

            if (includeGeolocation && testsDoneData.Any())
            {
                await FetchAndCacheGeolocationDataAsync(testsDoneData);
            }

            return testsDoneData;
        }

        /// <summary>
        /// Retrieves positivity rate data, including the country, week, positivity rate, population, with an option to include geolocation information.
        /// </summary>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation data in the results.</param>
        /// <returns>An asynchronous task that returns a collection of CovidDataDTO objects representing positivity rate data.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetPositivityRateAsync(bool includeGeolocation)
        {
            var positivityRateData = await _context.CovidData
                .GroupBy(d => new { d.Country, d.Week })
                .Select(group => new CovidDataDTO
                {
                    Country = group.Key.Country,
                    Week = group.Key.Week,
                    PositivityRate = (double)group.Sum(d => d.NewCases) / group.Sum(d => d.TestsDone),
                    Population = group.Max(d => d.Population),
                    Geolocation = null // Default to null, as it's not provided in this method
                })
                .ToListAsync();

            if (includeGeolocation && positivityRateData.Any())
            {
                await FetchAndCacheGeolocationDataAsync(positivityRateData);
            }

            return positivityRateData;
        }

        /// <summary>
        /// Retrieves testing rate data, including the country, week, testing rate, population, with an option to include geolocation information.
        /// </summary>
        /// <param name="includeGeolocation">A flag indicating whether to include geolocation data in the results.</param>
        /// <returns>An asynchronous task that returns a collection of CovidDataDTO objects representing testing rate data.</returns>
        public async Task<IEnumerable<CovidDataDTO>> GetTestingRateAsync(bool includeGeolocation)
        {
            var testingRateData = await _context.CovidData
                .GroupBy(d => new { d.Country, d.Week })
                .Select(group => new CovidDataDTO
                {
                    Country = group.Key.Country,
                    Week = group.Key.Week,
                    TestingRate = (double)group.Sum(d => d.TestsDone) / (group.Max(d => d.Population) / 100000), // Assuming population is in thousands
                    Population = group.Max(d => d.Population),
                    Geolocation = null // Default to null, as it's not provided in this method
                })
                .ToListAsync();

            if (includeGeolocation && testingRateData.Any())
            {
                await FetchAndCacheGeolocationDataAsync(testingRateData);
            }

            return testingRateData;
        }

        /// <summary>
        /// Retrieves a list of all countries.
        /// </summary>
        /// <returns>An asynchronous task that returns a collection of strings representing all countries.</returns>
        public async Task<IEnumerable<string>> GetAllCountriesAsync()
        {
            try
            {
                var countries = await _context.CovidData
                    .Select(data => data.Country)
                    .Distinct()
                    .ToListAsync();

                return countries;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                throw new ApplicationException($"An error occurred while fetching countries: {ex.Message}");
            }
        }




    }


}
