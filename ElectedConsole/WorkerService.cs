// Worker.cs of service 1.

using ET.FakeText;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public sealed class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IZooKeeperClient _zooKeeperClient;
    private readonly Setting _setting;
    private readonly string _name;

    public WorkerService(ILogger<WorkerService> logger, IZooKeeperClient zooKeeperClient, Setting setting)
    {
        _logger = logger;
        _zooKeeperClient = zooKeeperClient;
        _setting = setting;
        _name = setting.Name;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (await _zooKeeperClient.CheckLeaderAsync(_name))
                _logger.LogInformation($"{_name}:{_setting.RootNode} is processing... {DateTime.Now}");

            await Task.Delay(3000, token);
        }
    }
}