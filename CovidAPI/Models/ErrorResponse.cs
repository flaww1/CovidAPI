using System.Text.Json;

namespace CovidAPI.Models
{
    public class ErrorResponse
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

}
