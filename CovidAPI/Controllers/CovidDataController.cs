using System.Collections.Generic;
using System.Threading.Tasks;
using CovidAPI.Models;
using CovidAPI.Services.Rest;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/coviddata")]
public class CovidDataController : ControllerBase
{
    private readonly IGeolocationService _geolocationService; 

    private readonly ICovidDataService _covidDataService;

    public CovidDataController(ICovidDataService covidDataService, IGeolocationService geolocationService)
    {
        _covidDataService = covidDataService;
        _geolocationService = geolocationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetAllData()
    {
        var data = await _covidDataService.GetAllDataAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CovidDataDTO>> GetDataById(int id)
    {
        var data = await _covidDataService.GetDataByIdAsync(id);

        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult<CovidDataDTO>> AddData([FromBody] CovidDataDTO covidDataDTO)
    {
        await _covidDataService.AddDataAsync(covidDataDTO);
        return CreatedAtAction(nameof(GetDataById), new { id = covidDataDTO.Id }, covidDataDTO);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateData(int id, [FromBody] CovidDataDTO covidDataDTO)
    {
        if (id != covidDataDTO.Id)
        {
            return BadRequest();
        }

        await _covidDataService.UpdateDataAsync(covidDataDTO);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteData(int id)
    {
        _covidDataService.DeleteDataAsync(id);
        return NoContent();
    }

    [HttpGet("country/{country}")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetDataByCountry(string country)
    {
        try
        {
            var data = await _covidDataService.GetDataByCountryAsync(country);

            if (data != null && data.Any())
            {
                // Fetch geolocation information for each country individually
                foreach (var covidData in data)
                {
                    var geolocation = await _geolocationService.GetGeolocationInfoAsync(covidData.Country);
                    covidData.Geolocation = geolocation?.Results?.FirstOrDefault()?.Components;
                }

                return Ok(data);
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("year/{year}")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetDataByYear(int year)
    {
        var data = await _covidDataService.GetDataByYearAsync(year);
        return Ok(data);
    }

    [HttpGet("week/{week}")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetDataByWeek(string week)
    {
        var data = await _covidDataService.GetDataByWeekAsync(week);
        return Ok(data);
    }
}
