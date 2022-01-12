using Cncjs.Api.Models;
using CSharpFunctionalExtensions;

namespace Cncjs.Api;

public class Macro
{
    private readonly CncJsClient   _cncJs;
    private readonly CncJsSocketIo _client;

    private const string GetMacrosPath = "macros";
    private const string Command       = "command";
    private const string RunCommand    = "macro:run";

    public Macro(CncJsSocketIo client,CncJsClient cncJs)
    {
        _cncJs = cncJs;
        _client = client;
    }

    public Task<Result<MacroModel[]>> GetMacros()
    {
        return _cncJs.Get<Macros>(GetMacrosPath)
            .Map(m=>m.Records);
    }

    public async Task RunMacro(string port, string id)
    {
        await _client.EmitAsync(Command, port, RunCommand, id);
    }
}