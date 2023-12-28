using System.Text.Json;

namespace CovidAPI.Models
{
    /// <summary>
    /// Represents an error response containing information about an error.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the error code associated with the error.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error message providing details about the encountered error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Converts the object to its equivalent JSON representation.
        /// </summary>
        /// <returns>A JSON string representing the current object.</returns>
        public override string ToString()
        {
            // Using JsonSerializer to serialize the object to a JSON string
            return JsonSerializer.Serialize(this);
        }
    }
}
