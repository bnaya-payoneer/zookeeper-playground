public record Setting (string ConnectionString, int SessionTimeoutSec, string RootNode = "/leader-election");
