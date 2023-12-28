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
        // Assuming you have a method in your service or repository to check data existence
        public async Task<bool> DataExistsForCountryAndWeekAsync(string country, string week)
        {
            // Assuming you have a method in your repository to check if data exists
            // You need to replace it with your actual implementation
            var existingData = await GetDataByCountryAndWeekAsync(country, week);

            // Check if data exists
            return existingData != null;
        }

        public async Task<CovidData> GetDataByCountryAndWeekAsync(string country, string week)
        {
            // Assuming you have a DbContext named _context and a DbSet for CovidData
            // Replace this with your actual DbContext and DbSet
            return await _context.CovidData
                .FirstOrDefaultAsync(c => c.Country == country && c.Week == week);
        }

        // Asynchronous methods with DTOs
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






        public async Task AddDataAsync(CovidDataDTO covidDataDTO)
        {
            _context.CovidData.Add(MapToEntity(covidDataDTO));
            await _context.SaveChangesAsync();
        }

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

        public async Task DeleteDataAsync(int id)
        {
            var covidData = await _context.CovidData.FindAsync(id);
            if (covidData != null)
            {
                _context.CovidData.Remove(covidData);
                await _context.SaveChangesAsync();
            }
        }




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

     


    }


}
