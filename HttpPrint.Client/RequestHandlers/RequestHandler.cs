using Newtonsoft.Json;
using System.Net;
using HttpPrint.Client.Models;

namespace HttpPrint.Client.RequestHandlers
{
    public class RequestHandler
    {
        
        public static async Task<string> ProcessRequest(HttpListenerContext context)
        {
            var uri = context.Request.Url;
            try
            {

                switch (uri?.LocalPath.ToLower())
                {
                    case "/printerlist":
                    case "printerlist":
                        return JsonConvert.SerializeObject(await PrinterHelper.GetAllInstalledPrinters());
                    case "print":
                    case "/print":
                        return JsonConvert.SerializeObject(await PrintHandler.HandlePrintRequest(context));
                    default:
                        return JsonConvert.SerializeObject(PrintResponse.Failed("Invalid Request"));
                }
            }
            catch (Exception ex)
            {
                JsonConvert.SerializeObject(PrintResponse.Failed("Invalid Request" + ex.Message));
            }
            return JsonConvert.SerializeObject(PrintResponse.Failed("Request Not Supported"));
        }
    }
}
