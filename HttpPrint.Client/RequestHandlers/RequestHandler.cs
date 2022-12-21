using System;
using System.Net;

namespace HttpPrint.Client.RequestHandlers
{
    public class RequestHandler
    {
        private HttpListenerContext _context;
        public HttpListenerResponse Response;
        private Uri? _uri;
        
        protected RequestHandler(HttpListenerContext context)
        {

            _context = context;
            _uri = context.Request.Url;
            Response = context.Response;
            Response.StatusCode = 200;
            Response.StatusDescription = "OK";
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            Response.AddHeader("Access-Control-Allow-Methods", "POST,GET");
            Response.AddHeader("Access-Control-Max-Age", "1000");
            Response.AddHeader("Access-Control-Allow-Header", "Content-Type");
            Response.ContentType = "application/json; charset=utf-8";
        }

        public HttpListenerResponse HandleRequest()
        {
            Stream output = new MemoryStream();
            try
            {

                switch (_uri.LocalPath.ToLower())
                {
                    case "/printerlist":
                    case "printerlist":
                        output = GetAllPrinters(response);
                        break;
                    case "print":

                    default:
                        await HandlePrintRequest(context, response);
                        // must close the output stream.
                        break;
                }
            }
            catch (Exception ex)
            {
                response.ContentLength64 = buffer.Length;
                output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                // must close the output stream.
                output.Close();
            }
            _context.Response.Close();
            return Response;
        }

    }
}
