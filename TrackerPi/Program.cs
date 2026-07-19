using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrackerPi.Gps;
using TrackerPi.Services;


var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    services.AddHttpClient("GpsTracker", client =>
    {
        client.BaseAddress = new Uri(context.Configuration["ServerSettings:BaseUrl"]!);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    var gpsPort = context.Configuration["GpsSettings:SerialPort"] ?? "/dev/ttyUSB0";
    var baudRate = int.Parse(context.Configuration["GpsSettings:BaudRate"] ?? "4800");
    services.AddSingleton(new Sim7600GpsReader(gpsPort, baudRate));

    services.AddHostedService<GpsCollectionService>();
});

var host = builder.Build();
await host.RunAsync();