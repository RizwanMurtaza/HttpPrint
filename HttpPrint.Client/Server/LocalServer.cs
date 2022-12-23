using System.Net;
using System.Text;
using HttpPrint.Client.RequestHandlers;

namespace HttpPrint.Client.Server
{
    public class LocalServer
    {
        public static HttpListener? Listener;
        private int Port => 8888;

        public void Stop()
        {
            var ip = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
            NetAclChecker.RemoveAddress($"http://localhost:{Port}/");
            //NetAclChecker.RemoveAddress($"http://127.0.0.1:{Port}/");
            //NetAclChecker.RemoveAddress($"http://+:{Port}/");
            //NetAclChecker.RemoveAddress($"http://*:{Port}/");
            //NetAclChecker.RemoveAddress("http://" + ip + ":" + Port + " /");
            Listener.Stop();
        }
        public async Task Start()
        {
            var ip = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();

            NetAclChecker.AddAddress($"http://localhost:{Port}/");
            //NetAclChecker.AddAddress($"http://127.0.0.1:{Port}/");
            //NetAclChecker.AddAddress($"http://+:{Port}/");
            //NetAclChecker.AddAddress($"http://*:{Port}/");
            //NetAclChecker.AddAddress("http://" + ip + ":" + Port + " /");

            Listener = new HttpListener();
            Listener.Prefixes.Add($"http://localhost:{Port}/");
            //Listener.Prefixes.Add($"http://127.0.0.1:{Port}/");
            //Listener.Prefixes.Add($"http://+:{Port}/");
            //Listener.Prefixes.Add($"http://*:{Port}/");
            //Listener.Prefixes.Add($"http://{ip}:{Port}/");



            if (!HttpListener.IsSupported)
            {
                throw new Exception("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            }
            // Create a listener.
            Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            Listener.Start();
            await AcceptConnections().ConfigureAwait(false);
        }
        static async Task AcceptConnections()
        {
            Console.WriteLine("Waiting for connections");
            while (true)
            {
                HttpListenerContext ctx = await Listener.GetContextAsync().ConfigureAwait(false);
                await ListenerCallback(ctx);
            }
        }

        private static async Task ListenerCallback(HttpListenerContext context)
        {

            //var context = await Listener.GetContextAsync();
           
            Console.WriteLine(context.Request.Url);
            
            var response = context.Response;
            response.StatusCode = 200;
            response.StatusDescription = "OK";
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "POST,GET");
           
            var printResponse = await RequestHandler.ProcessRequest(context);
           
            var buffer = Encoding.ASCII.GetBytes(printResponse);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

    }
}
