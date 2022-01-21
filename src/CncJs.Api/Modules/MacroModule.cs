using CncJs.Api.Models;
using CSharpFunctionalExtensions;

namespace CncJs.Api.Modules;

public class MacroModule
{
    private readonly CncJsClient     _client;

    private const string GetMacrosPath = "macros";
    private const string Command       = "command";
    private const string RunCommand    = "macro:run";

    public MacroModule(CncJsClient client)
    {
        _client = client;
    }

    public Task<Result<Macro[]>> GetMacros()
    {
        return _client.SocketIoClient.Get<Macros>(GetMacrosPath)
            .Map(m=>m.Records);
    }

    public async Task RunMacro(string id, params object[] data)
    {
        if (!_client.ControllerModule.ControllerConnected && !_client.Connected)
            return;
        await _client.SocketIoClient.EmitAsync(Command, _client.ControllerModule.Controller.Port, RunCommand, id, data.Length == 1 ? data[0] : data);
    }
}