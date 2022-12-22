using HttpPrint.Client.Server;
using Topshelf;
HostFactory.Run(x =>
    {
        x.Service<LocalServer>(d=>
        {
            d.ConstructUsing(name => new LocalServer());
            d.WhenStarted(s => s.Start());
            d.WhenStopped(s => s.Stop());
            
        });
        x.RunAsLocalSystem();
        x.StartAutomatically();
        x.SetServiceName("PrintService");
        x.SetDescription("Local Print Server For Printing");
        x.StartAutomatically();
    }
);