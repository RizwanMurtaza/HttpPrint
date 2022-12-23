// See https://aka.ms/new-console-template for more information

using HttpPrint.Client.Server;

Console.WriteLine("Hello, World!");
var server = new LocalServer();
await server.Start();