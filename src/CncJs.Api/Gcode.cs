using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class Gcode
{
    private readonly CncJsSocketIo _client;

    // responses
    private const    string        Load   = "gcode:load";
    private const    string        UnLoad = "gcode:unload";

    // commands
    private const string GcodeCommand = "gcode";
    private const string Command = "command";

    public Func<Task> OnLoad { get; set; }
    public Func<Task> OnUnLoad { get; set; }

    internal Gcode(CncJsSocketIo client)
    {
        _client = client;

        _client.On(Load, _ => OnLoad?.Invoke());
        _client.On(UnLoad, _ => OnUnLoad?.Invoke());
    }

    public async Task SendCommandAsync(string port, string cmd)
    {
        await _client.EmitAsync(Command, port, GcodeCommand, cmd);
    }

}