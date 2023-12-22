// CovidDataController.cs
using CovidAPI.Models;
using CovidAPI.Services.Rest;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/coviddata")]
public class CovidDataController : ControllerBase
{
    private readonly ICovidDataService _covidDataService;

    public CovidDataController(ICovidDataService covidDataService)
    {
        _covidDataService = covidDataService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CovidData>> Get()
    {
        var data = _covidDataService.GetAllData();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public ActionResult<CovidData> GetById(int id)
    {
        var data = _covidDataService.GetDataById(id);

        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }

    [HttpPost]
    public ActionResult<CovidData> Post([FromBody] CovidData covidData)
    {
        _covidDataService.AddData(covidData);
        return CreatedAtAction(nameof(GetById), new { id = covidData.Id }, covidData);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] CovidData covidData)
    {
        if (id != covidData.Id)
        {
            return BadRequest();
        }

        _covidDataService.UpdateData(covidData);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _covidDataService.DeleteData(id);
        return NoContent();
    }


}
