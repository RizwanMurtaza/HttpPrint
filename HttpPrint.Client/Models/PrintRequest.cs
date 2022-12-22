using Newtonsoft.Json;
namespace HttpPrint.Client.Models
{
    public class PrintRequest
    {
        public Guid RequestId { get; set; }
        public string PrinterName { get; set; }

        public int? NumberOfCopies { get; set; }

        [JsonIgnore]
        public MemoryStream? FileStream {
            get => GetStream();
            set
            {

            }

        }
        public string Base64String { get; set; }

        public string? Url { get; set; }

        public bool IsUrlRequest { get; set; }

        private MemoryStream GetStream()
        {
            string converted = Base64String.Replace('-', '+');
            converted = converted.Replace('_', '/');
            var byteArray = System.Convert.FromBase64String(converted);
            MemoryStream stream = new MemoryStream(byteArray);
            stream.Position = 0;
            return stream;
        }
    }
}
