using Microsoft.Extensions.Hosting;


var builder = Host.CreateDefaultBuilder(args);

var host = builder.Build();
await host.RunAsync();