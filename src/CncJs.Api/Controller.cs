using Cncjs.Api.Models;
using SocketIOClient;

namespace Cncjs.Api;

public class Controller
{
    private readonly CncJsSocketIo _client;
    private const    string        Settings = "controller:settings";
    private const    string        State    = "controller:state";

    private const string Command = "command";
    private const string Home = "homing";
    private const string Unlock = "unlock";
    private const string Reset = "reset";
    private const string Feedhold = "feedhold";
    private const string Cyclestart = "cyclestart";

    public Action<ControllerSettings> OnSettings { get; set; }
    public Action<ControllerState> OnState { get; set; }
    internal Controller(CncJsSocketIo client)
    {
        _client = client;
        _client.On(Settings, OnSettingsEvent);
        _client.On(State, OnStateEvent);
    }
    private void OnSettingsEvent(SocketIOResponse obj)
    {
        var settings = new ControllerSettings
        {
            Type = obj.GetValue<string>()
        };
        settings.Settings = settings.Type switch
        {
            ControllerTypes.Grbl => Grbl.GetSettings(obj.GetValue(1)),
            _ => null
        };
        OnSettings?.Invoke(settings);
    }

    private void OnStateEvent(SocketIOResponse obj)
    {
        var state = new ControllerState
        {
            Type = obj.GetValue<string>(),
            State = obj.GetValue<State>(1)
        };
        OnState?.Invoke(state);
    }

    public async Task HomeAsync(string port)
    {
        await _client.EmitAsync(Command, port, Home);
    }

    public async Task ResetAsync(string port)
    {
        await _client.EmitAsync(Command, port, Reset);
    }

    public async Task UnlockAsync(string port)
    {
        await _client.EmitAsync(Command, port, Unlock);
    }

    public async Task FeedholdAsync(string port)
    {
        await _client.EmitAsync(Command, port, Feedhold);
    }

    public async Task CyclestartAsync(string port)
    {
        await _client.EmitAsync(Command, port, Cyclestart);
    }

}