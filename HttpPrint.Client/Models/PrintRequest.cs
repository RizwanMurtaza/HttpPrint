namespace HttpPrint.Client.Models
{
    public class PrintRequest
    {
        public Guid RequestId { get; set; }
        public string PrinterName { get; set; }

        public int? NumberOfCopies { get; set; }

        public Stream? FileStream { get; set; }

        public string? Url { get; set; }

        public bool IsUrlRequest { get; set; }
    }
}
