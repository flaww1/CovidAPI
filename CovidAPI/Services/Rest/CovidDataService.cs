using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CovidAPI.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CovidAPI.Services.Rest
{
    public class CovidDataService : ICovidDataService
    {
        private readonly IGeolocationService _geolocationService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CovidDataService> _logger;
        private readonly GeolocationCache _geolocationCache;

        public CovidDataService(ApplicationDbContext context, IGeolocationService geolocationService, ILogger<CovidDataService> logger, GeolocationCache geolocationCache)
        {
            _context = context;
            _geolocationService = geolocationService;
            _logger = logger;
            _geolocationCache = geolocationCache;
        }

        // Synchronous methods
        public IEnumerable<CovidData> GetAllData() => _context.CovidData.ToList();

        public CovidData GetDataById(int id) => _context.CovidData.Find(id);

        public IEnumerable<CovidData> GetDataByCountry(string country) =>
            _context.CovidData.Where(data => data.Country == country).ToList();

        public IEnumerable<CovidData> GetDataByYear(int year) =>
            _context.CovidData.Where(data => data.Year == year).ToList();

        public IEnumerable<CovidData> GetDataByWeek(int week) =>
            _context.CovidData.Where(data => data.Week == week.ToString()).ToList();

        public void AddData(CovidData covidData)
        {
            _context.CovidData.Add(covidData);
            _context.SaveChanges();
        }

        public void UpdateData(CovidData covidData)
        {
            _context.CovidData.Update(covidData);
            _context.SaveChanges();
        }

        public void DeleteData(int id)
        {
            var covidData = _context.CovidData.Find(id);
            if (covidData != null)
            {
                _context.CovidData.Remove(covidData);
                _context.SaveChanges();
            }
        }

        public double CalculatePositivityRate(int year, int week)
        {
            var dataForWeek = _context.CovidData
                .Where(data => data.Year == year && data.Week == week.ToString())
                .ToList();

            if (dataForWeek.Any())
            {
                var totalTestsDone = dataForWeek.Sum(data => data.TestsDone);
                var totalNewCases = dataForWeek.Sum(data => data.NewCases);

                if (totalTestsDone > 0)
                {
                    return ((double)totalNewCases / totalTestsDone) * 100;
                }
            }

            return 0; // Default to 0 if no data or tests done.
        }

        // Asynchronous methods with DTOs
        public async Task<IEnumerable<CovidDataDTO>> GetAllDataAsync() =>
            await _context.CovidData
                .Select(data => MapToDTO(data))
                .ToListAsync();

        public async Task<CovidDataDTO> GetDataByIdAsync(int id) =>
            MapToDTO(await _context.CovidData.FindAsync(id));

        public async Task<IEnumerable<CovidDataDTO>> GetDataByCountryAsync(string country) =>
            await _context.CovidData
                .Where(data => data.Country == country)
                .Select(data => MapToDTO(data))
                .ToListAsync();

        public async Task<IEnumerable<CovidDataDTO>> GetDataByYearAsync(int year) =>
            await _context.CovidData
                .Where(data => data.Year == year)
                .Select(data => MapToDTO(data))
                .ToListAsync();

        public async Task<IEnumerable<CovidDataDTO>> GetDataByWeekAsync(string week)
        {
            try
            {
                var data = await _context.CovidData
                    .Where(data => data.Week == week)
                    .Select(data => new CovidDataDTO
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
                    })
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                throw new ApplicationException($"An error occurred: {ex.Message}");
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

        public async Task<double> CalculatePositivityRateAsync(int year, string week)
        {
            var dataForWeek = await _context.CovidData
                .Where(data => data.Year == year && data.Week == week)
                .ToListAsync();

            if (dataForWeek.Any())
            {
                var totalTestsDone = dataForWeek.Sum(data => data.TestsDone);
                var totalNewCases = dataForWeek.Sum(data => data.NewCases);

                if (totalTestsDone > 0)
                {
                    return ((double)totalNewCases / totalTestsDone) * 100;
                }
            }

            return 0; // Default to 0 if no data or tests done.
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
                    // Fetch geolocation information for the unique countries
                    var uniqueCountries = data.Select(d => d.Country).Distinct().ToList();

                    foreach (var uniqueCountry in uniqueCountries)
                    {
                        // Check if the geolocation information is already in the cache
                        if (_geolocationCache.TryGetFromCache(uniqueCountry, out var cachedResponse))
                        {
                            // If cached, update geolocation in the data
                            UpdateGeolocationInData(data, uniqueCountry, cachedResponse);
                        }
                        else
                        {
                            // If not cached, fetch geolocation information from the service
                            var geolocationResponse = await _geolocationService.GetGeolocationInfoAsync(uniqueCountry);

                            // Update geolocation in the data
                            UpdateGeolocationInData(data, uniqueCountry, geolocationResponse);

                            // Add the response to the cache
                            _geolocationCache.AddToCache(uniqueCountry, geolocationResponse);
                        }
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                // Log the exception and return a meaningful response
                throw new ApplicationException($"An error occurred: {ex.Message}");
            }
        }


        private void UpdateGeolocationInData(IEnumerable<CovidDataDTO> data, string country, GeolocationApiResponse geolocationResponse)
        {
            foreach (var covidData in data.Where(d => d.Country == country))
            {
                // Log the data before mapping
                _logger.LogInformation($"Before Mapping: {JsonConvert.SerializeObject(covidData)}");

                // Find the matching result in the geolocation response
                var result = geolocationResponse.Results.FirstOrDefault(r => r.Components?.Country == country);

                // Check if a matching result was found
                if (result != null)
                {
                    // Assign the Geometry object to the CovidDataDTO object
                    covidData.Geometry = result.Geometry;

                    // Log the data after mapping
                    _logger.LogInformation($"After Mapping: {JsonConvert.SerializeObject(covidData.Geometry)}");
                }
                else
                {
                    _logger.LogWarning($"No geolocation data found for country {country}");
                }
            }
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
    }
}
