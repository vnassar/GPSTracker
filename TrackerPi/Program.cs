using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    services.AddHttpClient("GpsTracker", client =>
    {
        client.BaseAddress = new Uri(context.Configuration["ServerSettings:BaseUrl"]!);
        client.Timeout = TimeSpan.FromSeconds(10);
    });
});

var host = builder.Build();
await host.RunAsync();