using CncJs.Api.Models;
using CSharpFunctionalExtensions;

namespace CncJs.Api.Modules;

public class WatchModule
{
    private readonly CncJsClient     _client;

    private const string GetFilesPath = "watch/files";
    private const string QueryPath    = "path";

    internal WatchModule(CncJsClient client)
    {
        _client = client;
    }

    public Task<Result<WatchPath>> GetFiles(string path = "")
    {
        return _client.SocketIoClient.Get<WatchPath>(GetFilesPath, new KeyValuePair<string, string>(QueryPath, path));
    }
}