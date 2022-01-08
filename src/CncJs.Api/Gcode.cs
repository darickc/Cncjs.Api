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
    private const string SendCommand = "gcode";

    public Func<Task> OnLoad { get; set; }
    public Func<Task> OnUnLoad { get; set; }

    internal Gcode(CncJsSocketIo client)
    {
        _client = client;

        _client.On(Load, _ => OnLoad?.Invoke());
        _client.On(UnLoad, _ => OnUnLoad?.Invoke());
    }

    public async Task SendCommandAsync(string command)
    {
        await _client.EmitAsync(SendCommand, command);
    }

}