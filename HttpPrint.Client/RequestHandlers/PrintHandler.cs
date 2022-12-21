using HttpPrint.Client.Models;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace HttpPrint.Client.RequestHandlers
{
    public class PrintHandler
    {
        public static async Task<PrintResponse> HandlePrintRequest(HttpListenerContext context)
        {
            var requestBody = await new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEndAsync();
            var model = JsonConvert.DeserializeObject<PrintRequest>(requestBody);
            
            if (model == null)
            {
                return PrintResponse.Failed("Invalid Request body");
            }
            try
            {
                await HandlePrintRequest(model);
            }
            catch (Exception e)
            {
                return PrintResponse.Failed(e.Message);
            }
            return PrintResponse.Success(string.Empty);
        }

        public static async Task<PrintResponse> HandlePrintRequest(PrintRequest request)
        {
            var result = await ValidateRequest(request);
            if (!result.IsSuccess)
            {
                return result;
            }

            if (request.IsUrlRequest)
            {
                return await PrintFromUrl(request);
            }

            if (request.FileStream == null)
            {
                return PrintResponse.Failed("No File Provided to Print");
            }

            var numberOfCopies = request.NumberOfCopies ?? 1;
            return await PrinterHelper.PrintPdf(request.PrinterName, numberOfCopies,request.FileStream);
        }

        private static async Task<PrintResponse> ValidateRequest(PrintRequest request)
        {
            if (string.IsNullOrEmpty(request.PrinterName))
            {
                return PrintResponse.Failed("No Printer in request");

            }

            var allPrinterAvailable = await PrinterHelper.GetAllInstalledPrinters();

            var availablePrinterName = allPrinterAvailable.Where(z => z.ToLower().Equals(request.PrinterName.ToLower())).ToList();
            if (availablePrinterName.Any())
            {
                request.PrinterName = availablePrinterName.First();
            }
            else
            {
                return PrintResponse.Failed("Invalid Printer in request");

            }
            return PrintResponse.Success();

        }

        private static async Task<PrintResponse> PrintFromUrl(PrintRequest request)
        {
            try
            {
                var validUrl = new Uri(request.Url);
                using var client = new HttpClient();
                using var responseClient = await client.GetAsync(validUrl);
              
                if (!responseClient.IsSuccessStatusCode)
                    return PrintResponse.Failed("Failed to get file from URL");
                
                request.FileStream = await responseClient.Content.ReadAsStreamAsync();
                request.FileStream.Seek(0, SeekOrigin.Begin);
                return await PrinterHelper.PrintPdf(request.PrinterName, 1, request.FileStream);


            }
            catch (Exception ex)
            {
                return PrintResponse.Failed("Failed :-" + ex.Message);
            }
        }

        

    }
}
