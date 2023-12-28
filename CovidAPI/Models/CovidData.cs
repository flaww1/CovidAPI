namespace CovidAPI.Models
{
    /// <summary>
    /// Represents the Covid-19 data stored in the database.
    /// </summary>
    public class CovidData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Covid-19 data.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the country where the data belongs.
        /// </summary>
        public string? Country { get; set; } // Add "?" to make it nullable

        /// <summary>
        /// Gets or sets the country code for the data.
        /// </summary>
        public string? CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the year for which the data is recorded.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the week within the year for which the data is recorded.
        /// </summary>
        public string? Week { get; set; }

        /// <summary>
        /// Gets or sets the region where the data is recorded.
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Gets or sets the name of the region where the data is recorded.
        /// </summary>
        public string? RegionName { get; set; }

        /// <summary>
        /// Gets or sets the number of new cases reported.
        /// </summary>
        public int NewCases { get; set; }

        /// <summary>
        /// Gets or sets the number of tests conducted.
        /// </summary>
        public int TestsDone { get; set; }

        /// <summary>
        /// Gets or sets the population of the region.
        /// </summary>
        public int Population { get; set; }

        /// <summary>
        /// Gets or sets the rate of testing.
        /// </summary>
        public double TestingRate { get; set; }

        /// <summary>
        /// Gets or sets the positivity rate of the tests conducted.
        /// </summary>
        public double PositivityRate { get; set; }

        /// <summary>
        /// Gets or sets the source of testing data.
        /// </summary>
        public string? TestingDataSource { get; set; }
    }
}
