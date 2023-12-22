using CovidAPI.Models;

namespace CovidAPI.Services.Rest
{
    public interface ICovidDataService
    {
        IEnumerable<CovidData> GetAllData();
        CovidData GetDataById(int id);
        IEnumerable<CovidData> GetDataByCountry(string country);
        IEnumerable<CovidData> GetDataByYear(int year);
        IEnumerable<CovidData> GetDataByWeek(int week);
        void AddData(CovidData covidData);
        void UpdateData(CovidData covidData);
        void DeleteData(int id);
        double CalculatePositivityRate(int year, int week);
    }
}
