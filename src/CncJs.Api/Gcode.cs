using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class Gcode
{
    private readonly CncJsSocketIo _client;

    private const string JogPattern = "([A-Za-z]([-+0-9]))";
    private const string ZeroPattern = "([A-Za-z])";
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

    public async Task JogAsync(string port, string value, double distance, double feedrate)
    {
        var movementType = "G91";
        if (value.Contains("0"))
        {
            movementType = "G90";
        }

        var results = Regex.Matches(value, JogPattern);
        var c = string.Join("", results.Select(r =>  r.Groups[0].Value + (r.Groups[2].Value != "0" ? distance : ""))) ;

        await SendCommandAsync(port, $"$J={movementType} {c} F{feedrate}");
    }

    public async Task SetZeroAsync(string port, string workspace, string value)
    {
        var results = Regex.Matches(value, ZeroPattern);
        var c = string.Join("", results.Select(r => $"{r.Groups[0].Value}0"));
        await SendCommandAsync(port, $"G10 L20 {workspace} {c}");
    }

}