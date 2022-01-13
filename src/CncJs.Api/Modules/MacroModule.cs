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
        return _client.HttpClient.Get<Macros>(GetMacrosPath)
            .Map(m=>m.Records);
    }

    public async Task RunMacro(string id, params string[] data)
    {
        await _client.SocketIoClient.EmitAsync(Command, _client.Controller.Port, RunCommand, id, data);
    }
}