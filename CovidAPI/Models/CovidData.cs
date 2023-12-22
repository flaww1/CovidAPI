namespace CovidAPI.Models
{
    // CovidData.cs
    public class CovidData
    {
        public int Id { get; set; }
        public string? Country { get; set; } // Add "?" to make it nullable

        public string? CountryCode { get; set; }
        public int Year { get; set; }
        public string? Week { get; set; }
        public string? Region { get; set; }
        public string? RegionName { get; set; }
        public int NewCases { get; set; }
        public int TestsDone { get; set; }
        public int Population { get; set; }
        public decimal TestingRate { get; set; }
        public decimal PositivityRate { get; set; }
        public string? TestingDataSource{ get; set; }

    
    }

}
