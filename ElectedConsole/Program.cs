// See https://aka.ms/new-console-template for more information
using ET.FakeText;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

TextGenerator ng = new TextGenerator(WordTypes.Name);
string name = ng.GenerateWord(10);

Console.WriteLine($"{name} is Starting");

Console.WriteLine("""
    Select Leader Group 0-9
    """);
var group = Console.ReadKey(true).KeyChar;

var hostBuilder = new HostBuilder()
  .ConfigureServices(services =>
     services.AddSingleton<IZooKeeperClient, ZooKeeperClient>()
             .AddSingleton(new Setting(name, "localhost:21811", 5, $"/leader-election/{group}"))
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

