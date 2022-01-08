using Cncjs.Api.Models;
using CSharpFunctionalExtensions;

namespace Cncjs.Api;

public class Watch
{
    private readonly CncJsSocketIo _client;
    private readonly CncJs         _cncJs;

    private const string State = "workflow:state";

    private const string GetFilesPath = "watch/files";
    private const string QueryPath    = "path";

    public Action OnState { get; set; }
    internal Watch(CncJsSocketIo client, CncJs cncJs)
    {
        _client = client;
        _cncJs = cncJs;
        _client.On(State, response => OnState?.Invoke());
    }

    public Task<Result<WatchPath>> GetFiles(string path = "")
    {
        return _cncJs.Get<WatchPath>(GetFilesPath, new KeyValuePair<string, string>(QueryPath, path));
    }
}