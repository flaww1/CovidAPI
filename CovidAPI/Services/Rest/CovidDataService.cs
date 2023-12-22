using System;
using System.Collections.Generic;
using System.Linq;
using CovidAPI.Models;
using CovidAPI.Services.Rest;
using Microsoft.EntityFrameworkCore;

public class CovidDataService : ICovidDataService
{
    private readonly ApplicationDbContext _context;

    public CovidDataService(ApplicationDbContext context)
    {
        _context = context;
    }

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

    
}
