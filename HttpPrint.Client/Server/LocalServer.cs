using System.Net;
using System.Text;
using HttpPrint.Client.RequestHandlers;

namespace HttpPrint.Client.Server
{
    public class LocalServer
    {
        public static HttpListener? Listener;
        private Thread? _listenerThread;
        private int Port => 8888;

        public void Start()
        {
            var ip = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();

            NetAclChecker.AddAddress($"http://localhost:{Port}/");
            NetAclChecker.AddAddress($"http://127.0.0.1:{Port}/");
            NetAclChecker.AddAddress($"http://+:{Port}/");
            NetAclChecker.AddAddress($"http://*:{Port}/");
            NetAclChecker.AddAddress("http://" + ip + ":" + Port + " /");

            Listener = new HttpListener();
            Listener.Prefixes.Add($"http://localhost:{Port}/");
            Listener.Prefixes.Add($"http://127.0.0.1:{Port}/");
            Listener.Prefixes.Add($"http://+:{Port}/");
            Listener.Prefixes.Add($"http://*:{Port}/");
            Listener.Prefixes.Add($"http://{ip}:{Port}/");



            if (!HttpListener.IsSupported)
            {
                throw new Exception("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            }
            // Create a listener.
            Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            Listener.Start();

           
            _listenerThread = new Thread(StartListener!);
            _listenerThread.Start();
        }
        private static void StartListener(object s)
        {
            while (true)
            {
                ProcessRequest();
            }
        }

        private static void ProcessRequest()
        {
            var result = Listener.BeginGetContext(ListenerCallback, Listener);
            result.AsyncWaitHandle.WaitOne();
        }


        private static async void ListenerCallback(IAsyncResult result)
        {

            var context = Listener.EndGetContext(result);
            var response = context.Response;
            response.StatusCode = 200;
            response.StatusDescription = "OK";
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "POST,GET");
            response.AddHeader("Access-Control-Max-Age", "1000");
            response.AddHeader("Access-Control-Allow-Header", "Content-Type");
            response.ContentType = "application/json; charset=utf-8";

            var printResponse = await RequestHandler.ProcessRequest(context);
           
            var buffer = Encoding.ASCII.GetBytes(printResponse);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

    }
}
