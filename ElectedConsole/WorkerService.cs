// Worker.cs of service 1.

using ET.FakeText;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public sealed class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IZooKeeperClient _zooKeeperClient;
    private readonly string _name;

    public WorkerService(ILogger<WorkerService> logger, IZooKeeperClient zooKeeperClient)
    {
        _logger = logger;
        _zooKeeperClient = zooKeeperClient;
        TextGenerator ng = new TextGenerator(WordTypes.Name);
        _name = ng.GenerateWord(10);
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (await _zooKeeperClient.CheckLeaderAsync(_name))
                _logger.LogInformation($"Processing... {DateTime.Now}");

            await Task.Delay(1000, token);
        }
    }
}