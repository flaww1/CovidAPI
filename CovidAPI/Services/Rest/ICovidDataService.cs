using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CovidAPI.Models;
using CovidAPI.Services.Rest;
using Microsoft.EntityFrameworkCore;


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

        Task<GeolocationApiResponse> GetGeolocationInfoAsync(string country);


    }
}