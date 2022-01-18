using CncJs.Api.Models;
using CSharpFunctionalExtensions;

namespace CncJs.Api.Modules;

public class WatchModule
{
    private readonly CncJsClient     _client;

    private const string GetFilesPath = "watch/files";
    private const string QueryPath    = "path";
    private const string Load         = "watchdir:load";
    private const string Command      = "command";

    public WatchFile[] Files { get; set; }

    internal WatchModule(CncJsClient client)
    {
        _client = client;
    }

    public Task<Result<WatchFile[]>> GetFiles(string path = "")
    {
        return _client.SocketIoClient.Get<WatchPath>(GetFilesPath, new KeyValuePair<string, string>(QueryPath, path)) 
            .Tap(p =>
            {
                foreach (var file in p.Files)
                {
                    file.Path = string.Join("/", p.Path, file.Name);
                }
            })
            .TapIf(p => string.IsNullOrEmpty(p.Path), p => Files = p.Files)
            .Map(p=> p.Files);
    }

    public async Task LoadFile(WatchFile file)
    {
        if(_client.ControllerModule.ControllerConnected)
            await _client.SocketIoClient.EmitAsync(Command, _client.ControllerModule.Controller.Port, Load, file.Path);
    }
}