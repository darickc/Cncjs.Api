using CncJs.Api.Models;
using CSharpFunctionalExtensions;

namespace CncJs.Api.Modules;

public class WatchModule
{
    private readonly CncJsClient     _client;

    private const string State = "workflow:state";

    private const string GetFilesPath = "watch/files";
    private const string QueryPath    = "path";

    public event EventHandler OnState;
    internal WatchModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(State, response => OnState?.Invoke(this, EventArgs.Empty));
    }

    public Task<Result<WatchPath>> GetFiles(string path = "")
    {
        return _client.HttpClient.Get<WatchPath>(GetFilesPath, new KeyValuePair<string, string>(QueryPath, path));
    }
}