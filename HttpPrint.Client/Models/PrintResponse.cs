using System.Reflection.Metadata.Ecma335;

namespace HttpPrint.Client.Models
{
    public class PrintResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }

        public string? Output { get; set; }
        public static PrintResponse Success(string? output = null)
        {
            return new PrintResponse()
            {
                IsSuccess = true,
                Output = output
            };
        }

        public static PrintResponse Failed(string message)
        {
            return new PrintResponse()
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
