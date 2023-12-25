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
    public async Task<IEnumerable<CovidDataDTO>> GetAllDataAsync()
    {
        var data = await _covidDataService.GetAllDataAsync(includeGeolocation: true);
        return data;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CovidDataDTO>> GetDataById(int id)
    {
        var data = await _covidDataService.GetDataByIdAsync(id, includeGeolocation: true);

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
            var data = await _covidDataService.GetDataByCountryAsync(country, includeGeolocation: true);

            if (data != null && data.Any())
            {
                // Fetch geolocation information for each country individually
                foreach (var covidData in data)
                {
                    var geolocation = await _geolocationService.GetGeolocationInfoAsync(covidData.Country);
                    covidData.Geolocation = geolocation?.Results?.FirstOrDefault()?.Components;
                    covidData.Geometry = geolocation?.Results?.FirstOrDefault()?.Geometry;

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
        var data = await _covidDataService.GetDataByYearAsync(year, includeGeolocation: true);
        return Ok(data);
    }

    [HttpGet("week/{week}")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetDataByWeek(string week)
    {
        var data = await _covidDataService.GetDataByWeekAsync(week, includeGeolocation: true);
        return Ok(data);
    }
    [HttpGet("weeks")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllWeeks()
    {
        try
        {
            var weeks = await _covidDataService.GetAllWeeksAsync();
            return Ok(weeks);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
    [HttpGet("total-cases")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetTotalCases()
    {
        var data = await _covidDataService.GetTotalCasesAsync(includeGeolocation: true);
        return Ok(data);
    }

    [HttpGet("new-cases")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetNewCases()
    {
        var data = await _covidDataService.GetNewCasesAsync(includeGeolocation: true);
        return Ok(data);
    }

    [HttpGet("total-tests")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetTotalTests()
    {
        var data = await _covidDataService.GetTotalTestsAsync(includeGeolocation: true);
        return Ok(data);
    }

    [HttpGet("testing-rate")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetTestingRate()
    {
        var data = await _covidDataService.GetTestingRateAsync(includeGeolocation: true);
        return Ok(data);
    }

    [HttpGet("positivity-rate")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetPositivityRate()
    {
        var data = await _covidDataService.GetPositivityRateAsync(includeGeolocation: true);
        return Ok(data);
    }

    [HttpGet("geolocation")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetGeolocation()
    {
        var data = await _covidDataService.GetGeolocationAsync(includeGeolocation: true);
        return Ok(data);
    }

    [HttpGet("testing-source")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetTestingSource()
    {
        var data = await _covidDataService.GetTestingSourceAsync(includeGeolocation: true);
        return Ok(data);
    }

    [HttpGet("compare")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetComparisons([FromQuery] List<string> countries)
    {
        var data = await _covidDataService.GetComparisonsAsync(countries, includeGeolocation: true);
        return Ok(data);
    }

    [HttpGet("testing-rate-info")]
    public async Task<ActionResult<string>> GetTestingRateInfo()
    {
        // Add logic to provide information about testing rate interpretation
        return Ok("Testing rate represents...");
    }

    [HttpGet("population")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetPopulationData()
    {
        var data = await _covidDataService.GetPopulationDataAsync(includeGeolocation: true);
        return Ok(data);
    }


}
