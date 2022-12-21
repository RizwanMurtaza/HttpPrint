using System.Net;
using System.Text;
using HttpPrint.Client.Models;
using Newtonsoft.Json;

namespace HttpPrint.Client.HttpListener
{
    public class HttpListner
    {
        public static System.Net.HttpListener Listener;
        public byte[] buffer;
        private Thread listenThread1;
        private int Port => 8888;

        public void Start()
        {
            string ip = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();

            NetAclChecker.AddAddress($"http://localhost:{Port}/");
            NetAclChecker.AddAddress($"http://127.0.0.1:{Port}/");
            NetAclChecker.AddAddress($"http://+:{Port}/");
            NetAclChecker.AddAddress($"http://*:{Port}/");
            NetAclChecker.AddAddress("http://" + ip + ":" + Port + " /");

            Listener = new System.Net.HttpListener();
            Listener.Prefixes.Add($"http://localhost:{Port}/");
            Listener.Prefixes.Add($"http://127.0.0.1:{Port}/");
            Listener.Prefixes.Add($"http://+:{Port}/");
            Listener.Prefixes.Add($"http://*:{Port}/");
            Listener.Prefixes.Add($"http://{ip}:{Port}/");



            if (!System.Net.HttpListener.IsSupported)
            {
                throw new Exception("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            }
            // Create a listener.
            Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            Listener.Start();

           
            listenThread1 = new Thread(new ParameterizedThreadStart(StartListener));
            listenThread1.Start();
        }
        private async void StartListener(object s)
        {
            while (true)
            {
                var result = Listener.BeginGetContext(ListenerCallback, Listener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        private async void ListenerCallback(IAsyncResult result)
        {
            HttpListenerContext context = Listener.EndGetContext(result);
            Uri uri = context.Request.Url;

            HttpListenerResponse response = context.Response;
            response.StatusCode = 200;
            response.StatusDescription = "OK";
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "POST,GET");
            response.AddHeader("Access-Control-Max-Age", "1000");
            response.AddHeader("Access-Control-Allow-Header", "Content-Type");
            response.ContentType = "application/json; charset=utf-8";
            //append the data response
           
            Stream output = new MemoryStream();
            Console.WriteLine("setup completed...");
            try
            {
                switch (uri.LocalPath.ToLower())
                {
                    case "/printerlist":
                    case "printerlist":
                        //get the printer list to show
                        output = GetAllPrinters(response);
                        break;
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
            context.Response.Close();
        }

        private async Task HandlePrintRequest(HttpListenerContext context, HttpListenerResponse response)
        {
            string requestBody = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();
            var model = JsonConvert.DeserializeObject<PrintRequest>(requestBody);
            var printService = new PrintService();

            try
            {
                await printService.HandlePrintRequest(model);
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                buffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(e.Message));
                throw;
            }
        }

        private Stream GetAllPrinters(HttpListenerResponse response)
        {
            buffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(PrinterHelper.GetAllInstalledPrinters()));
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            return output;
        }
    }
}
