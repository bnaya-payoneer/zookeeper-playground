// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("Starting");


var hostBuilder = new HostBuilder()
  .ConfigureServices(services =>
     services.AddSingleton<IZooKeeperClient, ZooKeeperClient>()
             .AddSingleton(new Setting("localhost:21811", 5))
             .AddHostedService<WorkerService>())
             .ConfigureLogging(logging =>
             {
                 logging.AddConsole();
             });

CancellationTokenSource cts = new CancellationTokenSource();
var token = cts.Token;
Console.CancelKeyPress += OnCancel;

void OnCancel(object? sender, ConsoleCancelEventArgs e)
{
    Console.CancelKeyPress -= OnCancel;
    cts.Cancel();
}

var host = hostBuilder.Build();
await host.StartAsync(token);
Console.WriteLine("Started");
await host.WaitForShutdownAsync(token);

