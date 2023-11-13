public interface IZooKeeperClient: IDisposable
{
    Task<bool> CheckLeaderAsync(string hostName);
}