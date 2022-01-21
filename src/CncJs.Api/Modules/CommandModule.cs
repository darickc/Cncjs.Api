using CncJs.Api.Models;
using CSharpFunctionalExtensions;

namespace CncJs.Api.Modules;

public class CommandModule
{
    private readonly CncJsClient _client;

    private const string GetCommandsPath = "commands";
    private const string Command       = "command";
    private const string RunCommandPath    = "commands/run/";

    public CommandModule(CncJsClient client)
    {
        _client = client;
    }

    public Task<Result<Command[]>> GetCommands()
    {
        return _client.SocketIoClient.Get<Commands>(GetCommandsPath)
            .Map(m => m.Records);
    }

    public async Task Run(string id, params object[] data)
    {
        await _client.SocketIoClient.Post(RunCommandPath + id, null);
    }
}