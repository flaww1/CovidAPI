using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CovidAPI.Models;
using CovidAPI.Services.Rest;
using Microsoft.EntityFrameworkCore;


namespace CovidAPI.Services.Rest
{
    public class CovidDataService : ICovidDataService
    {
        private readonly IGeolocationService _geolocationService; 

        private readonly ApplicationDbContext _context;

        public CovidDataService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Synchronous methods
        public IEnumerable<CovidData> GetAllData() => _context.CovidData.ToList();
        public CovidData GetDataById(int id) => _context.CovidData.Find(id);
        public IEnumerable<CovidData> GetDataByCountry(string country) => _context.CovidData.Where(data => data.Country == country).ToList();
        public IEnumerable<CovidData> GetDataByYear(int year) => _context.CovidData.Where(data => data.Year == year).ToList();
        public IEnumerable<CovidData> GetDataByWeek(int week) => _context.CovidData.Where(data => data.Week == week.ToString()).ToList();
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

        public async Task<IEnumerable<CovidDataDTO>> GetDataByWeekAsync(string week) =>
            await _context.CovidData
                .Where(data => data.Week == week)
                .Select(data => MapToDTO(data))
                .ToListAsync();

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

        private CovidDataDTO MapToDTO(CovidData data)
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
                TestingDataSource = data.TestingDataSource
            };
        }

        public async Task<GeolocationApiResponse> GetGeolocationInfoAsync(string country) =>
    await _geolocationService.GetGeolocationInfoAsync(country);


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
