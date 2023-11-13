using org.apache.zookeeper;
using org.apache.zookeeper.recipes.leader;
using static org.apache.zookeeper.Watcher.Event;
using static org.apache.zookeeper.ZooDefs;
public sealed class ZooKeeperClient : IZooKeeperClient
{
    private readonly Setting _setting;
    private ZooKeeper? _zookeeper;
    private LeaderElectionSupport? _leaderElection;

    public ZooKeeperClient(Setting setting)
    {
        _setting = setting;
    }

    public async Task<bool> CheckLeaderAsync(string hostName)
    {
        if (_leaderElection is null)
        {
            var watcher = new ConnectionWatcher();
            _zookeeper = new ZooKeeper(_setting.ConnectionString, _setting.SessionTimeoutSec * 1000, watcher);

            await watcher.WaitForConnectionAsync();

            string rootNode = _setting.RootNode;
            if (await _zookeeper.existsAsync(rootNode) is null)
                await _zookeeper.createAsync(rootNode, Array.Empty<byte>(), Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            _leaderElection = new LeaderElectionSupport(_zookeeper, rootNode, hostName);
            await _leaderElection.start();
        }

        var leaderHostName = await _leaderElection.getLeaderHostName();
        return leaderHostName == hostName;
    }

    public void Dispose()
    {
        if (_leaderElection is not null)
            _leaderElection.stop().Wait();

        if (_zookeeper is not null)
            _zookeeper.closeAsync().Wait();
    }

    private sealed class ConnectionWatcher : Watcher
    {
        private readonly TaskCompletionSource _tcs = new();
        public Task WaitForConnectionAsync() => _tcs.Task;

        public override Task process(WatchedEvent @event)
        {
            if (@event.getState() is KeeperState.SyncConnected)
                _tcs.TrySetResult();

            return Task.CompletedTask;
        }
    }
}