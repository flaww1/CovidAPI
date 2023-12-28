using System.Collections.Generic;
using System.Threading.Tasks;
using CovidAPI.Models;
using CovidAPI.Services.Rest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for COVID-19 data-related operations.
/// </summary>
[ApiController]
[Route("api/coviddata")]
public class CovidDataController : ControllerBase
{
    private readonly IGeolocationService _geolocationService; 

    private readonly ICovidDataService _covidDataService;


    /// <summary>
    /// Initializes a new instance of the <see cref="CovidDataController"/> class.
    /// </summary>
    /// <param name="covidDataService">The COVID-19 data service.</param>
    /// <param name="geolocationService">The geolocation service.</param>
    public CovidDataController(ICovidDataService covidDataService, IGeolocationService geolocationService)
    {
        _covidDataService = covidDataService;
        _geolocationService = geolocationService;
    }


    /// <summary>
    /// Gets all COVID-19 data asynchronously.
    /// </summary>
    /// <returns>Returns a collection of COVID-19 data.</returns>
    [HttpGet]
    public async Task<IEnumerable<CovidDataDTO>> GetAllDataAsync()
    {
        var data = await _covidDataService.GetAllDataAsync(includeGeolocation: true);
        return data;
    }

    // <summary>
    /// Gets COVID-19 data by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the COVID-19 data.</param>
    /// <returns>Returns COVID-19 data with the specified ID.</returns>
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


    /// <summary>
    /// Adds new COVID-19 data.
    /// </summary>
    /// <param name="covidDataDTO">The COVID-19 data to add.</param>
    /// <returns>Returns the added COVID-19 data.</returns>
  //  [Authorize]
    [HttpPost]
    public async Task<ActionResult<CovidDataDTO>> AddData([FromBody] CovidDataDTO covidDataDTO)
    {
        // Check if data already exists for the given country and week
        if (await _covidDataService.DataExistsForCountryAndWeekAsync(covidDataDTO.Country, covidDataDTO.Week))
        {
            // Return a conflict response or handle it as per your application's logic
            return Conflict($"Data already exists for {covidDataDTO.Country} in week {covidDataDTO.Week}");
        }

        // Data doesn't exist, proceed to add new data
        await _covidDataService.AddDataAsync(covidDataDTO);

        return CreatedAtAction(nameof(GetDataById), new { id = covidDataDTO.Id }, covidDataDTO);
    }

    /// <summary>
    /// Updates COVID-19 data by ID.
    /// </summary>
    /// <param name="id">The ID of the COVID-19 data to update.</param>
    /// <param name="covidDataDTO">The updated COVID-19 data.</param>
    /// <returns>Returns no content if the update is successful.</returns>
    // [Authorize]
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

    /// <summary>
    /// Deletes COVID-19 data by ID.
    /// </summary>
    /// <param name="id">The ID of the COVID-19 data to delete.</param>
    /// <returns>Returns no content if the deletion is successful.</returns>
    // [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteData(int id)
    {
        await _covidDataService.DeleteDataAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Gets COVID-19 data by country.
    /// </summary>
    /// <param name="country">The name of the country.</param>
    /// <returns>Returns COVID-19 data for the specified country.</returns>
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

    /// <summary>
    /// Gets COVID-19 data by year.
    /// </summary>
    /// <param name="year">The year for which to retrieve data.</param>
    /// <returns>Returns COVID-19 data for the specified year.</returns>
    [HttpGet("year/{year}")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetDataByYear(int year)
    {
        var data = await _covidDataService.GetDataByYearAsync(year, includeGeolocation: true);
        return Ok(data);
    }

    /// <summary>
    /// Gets COVID-19 data by week.
    /// </summary>
    /// <param name="week">The week for which to retrieve data.</param>
    /// <returns>Returns COVID-19 data for the specified week.</returns>
    [HttpGet("week/{week}")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetDataByWeek(string week)
    {
        var data = await _covidDataService.GetDataByWeekAsync(week, includeGeolocation: true);
        return Ok(data);
    }

    /// <summary>
    /// Gets all available weeks for COVID-19 data.
    /// </summary>
    /// <returns>Returns a collection of all available weeks.</returns>
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

    /// <summary>
    /// Gets total COVID-19 cases.
    /// </summary>
    /// <returns>Returns total COVID-19 cases.</returns>
    [HttpGet("total-cases")]
    public async Task<ActionResult<IEnumerable<CovidDataDTO>>> GetTotalCases()
    {
        var data = await _covidDataService.GetTotalCasesAsync(includeGeolocation: true);
        return Ok(data);
    }



    /// <summary>
    /// Gets a list of all countries with available COVID-19 data.
    /// </summary>
    /// <returns>Returns a list of all countries with available COVID-19 data.</returns>
    [HttpGet("countries")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllCountries()
    {
        try
        {
            var countries = await _covidDataService.GetAllCountriesAsync();

            if (countries != null && countries.Any())
            {
                return Ok(countries);
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }



}
