using System.Text.RegularExpressions;
using CncJs.Api.Models;

namespace CncJs.Api.Modules;

public class GcodeModule
{
    private readonly CncJsClient _client;

    private const string JogPattern = "([A-Za-z]([-+0-9]))";
    private const string ZeroPattern = "([A-Za-z])";
    // responses
    private const string Load = "gcode:load";
    private const string UnLoad = "gcode:unload";

    // commands
    private const string GcodeCommand = "gcode";
    private const string Command = "command";

    public Gcode Gcode { get; set; }

    public event EventHandler<Gcode> OnLoad;
    public event EventHandler OnUnLoad;

    internal GcodeModule(CncJsClient client)
    {
        _client = client;

        _client.SocketIoClient.On(Load, response =>
        {
            Gcode = new Gcode(response.GetValue<string>(), response.GetValue<string>(1));
            OnLoad?.Invoke(this, Gcode);
            _client.OnPropertyChanged("Gcode");
        });
        _client.SocketIoClient.On(UnLoad, _ =>
        {
            Gcode = null;
            OnUnLoad?.Invoke(this, EventArgs.Empty);
            _client.OnPropertyChanged("Gcode");
        });
    }

    public async Task SendCommandAsync(string cmd)
    {
        if (!_client.ControllerModule.ControllerConnected && !_client.Connected)
            return;
        await _client.SocketIoClient.EmitAsync(Command, _client.ControllerModule.Controller.Port, GcodeCommand, cmd);
    }

    public async Task JogAsync(string value, double distance, double feedrate)
    {
        var movementType = "G91";
        if (value.Contains("0"))
        {
            movementType = "G90";
        }

        var results = Regex.Matches(value, JogPattern);
        var c = string.Join("", results.Select(r => r.Groups[0].Value + (r.Groups[2].Value != "0" ? distance : "")));

        await SendCommandAsync($"$J={movementType} {c} F{feedrate}");
    }

    public async Task SetZeroAsync(string workspace, string value)
    {
        var results = Regex.Matches(value, ZeroPattern);
        var c = string.Join("", results.Select(r => $"{r.Groups[0].Value}0"));
        await SendCommandAsync($"G10 L20 {workspace} {c}");
    }

    public async Task CancelJogAsync()
    {
        await _client.SerialPortModule.SendRawAsync("\x85;\n");
    }

}