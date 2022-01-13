using CncJs.Api.Models;
using SocketIOClient;

namespace CncJs.Api.Modules;

public class ControllerModule
{
    private readonly CncJsClient _client;
    private const    string      Settings = "controller:settings";
    private const    string      State    = "controller:state";

    private const string Command = "command";
    private const string Home = "homing";
    private const string Unlock = "unlock";
    private const string Reset = "reset";
    private const string Feedhold = "feedhold";
    private const string Cyclestart = "cyclestart";

    public event EventHandler<ControllerSettings> OnSettings;
    public event EventHandler<ControllerState> OnState;
    internal ControllerModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(Settings, OnSettingsEvent);
        _client.SocketIoClient.On(State, OnStateEvent);
    }
    private void OnSettingsEvent(SocketIOResponse obj)
    {
        var settings = new ControllerSettings
        {
            Type = obj.GetValue<string>()
        };
        settings.Settings = settings.Type switch
        {
            ControllerTypes.Grbl => GrblModule.GetSettings(obj.GetValue(1)),
            _ => null
        };
        OnSettings?.Invoke(this,settings);
    }

    private void OnStateEvent(SocketIOResponse obj)
    {
        var state = new ControllerState
        {
            Type = obj.GetValue<string>(),
            State = obj.GetValue<State>(1)
        };
        OnState?.Invoke(this,state);
    }

    public async Task HomeAsync()
    {
        if(_client.ControllerConnected)
            await _client.SocketIoClient.EmitAsync(Command, _client.Controller.Port, Home);
    }

    public async Task ResetAsync()
    {
        if (_client.ControllerConnected)
            await _client.SocketIoClient.EmitAsync(Command, _client.Controller.Port, Reset);
    }

    public async Task UnlockAsync()
    {
        if (_client.ControllerConnected)
            await _client.SocketIoClient.EmitAsync(Command, _client.Controller.Port, Unlock);
    }

    public async Task FeedholdAsync()
    {
        if (_client.ControllerConnected)
            await _client.SocketIoClient.EmitAsync(Command, _client.Controller.Port, Feedhold);
    }

    public async Task CyclestartAsync()
    {
        if (_client.ControllerConnected)
            await _client.SocketIoClient.EmitAsync(Command, _client.Controller.Port, Cyclestart);
    }

}