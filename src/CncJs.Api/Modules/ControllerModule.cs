using CncJs.Api.Models;
using SocketIOClient;

namespace CncJs.Api.Modules;

public class ControllerModule
{
    private readonly CncJsClient _client;
    private const    string      Settings = "controller:settings";
    private const    string      State    = "controller:state";
    private const    string      Change   = "serialport:change";
    private const    string      Open     = "serialport:open";
    private const    string      Close    = "serialport:close";
    private const    string      Error    = "serialport:error";
    
    private const string Command = "command";
    private const string Home = "homing";
    private const string Unlock = "unlock";
    private const string Reset = "reset";
    private const string Feedhold = "feedhold";
    private const string Cyclestart = "cyclestart";
   
    public bool ControllerConnected => Controller != null;

    public Controller Controller { get; private set; }

    public ControllerSettings ControllerSettings { get; private set; }

    public ControllerState ControllerState { get; private set; }

    public event EventHandler<ControllerSettings> OnSettings;
    public event EventHandler<ControllerState> OnState;
    public event EventHandler<Controller> OnChange;
    public event EventHandler<Controller> OnOpen;
    public event EventHandler<Controller> OnClose;
    public event EventHandler<Controller> OnError;

    internal ControllerModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(Settings, OnSettingsEvent);
        _client.SocketIoClient.On(State, OnStateEvent);

        _client.SocketIoClient.On(Change, response =>
        {
            Clear();
            Controller = response.GetValue<Controller>();
            OnChange?.Invoke(this, Controller);
        });
        _client.SocketIoClient.On(Open, response =>
        {
            Controller = response.GetValue<Controller>();
            OnOpen?.Invoke(this, Controller);
        });
        _client.SocketIoClient.On(Close, response =>
        {
            Clear();
            OnClose?.Invoke(this, response.GetValue<Controller>());
        });
        _client.SocketIoClient.On(Error, response => OnError?.Invoke(this, response.GetValue<Controller>()));
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
        ControllerSettings = settings;
        OnSettings?.Invoke(this,settings);
    }

    private void OnStateEvent(SocketIOResponse obj)
    {
        var state = new ControllerState
        {
            Type = obj.GetValue<string>(),
            State = obj.GetValue<State>(1)
        };
        ControllerState = state;
        OnState?.Invoke(this,state);
    }

    public async Task HomeAsync()
    {
        if(ControllerConnected && _client.Connected)
            await _client.SocketIoClient.EmitAsync(Command, Controller.Port, Home);
    }

    public async Task ResetAsync()
    {
        if (ControllerConnected && _client.Connected)
            await _client.SocketIoClient.EmitAsync(Command, Controller.Port, Reset);
    }

    public async Task UnlockAsync()
    {
        if (ControllerConnected && _client.Connected)
            await _client.SocketIoClient.EmitAsync(Command, Controller.Port, Unlock);
    }

    public async Task FeedholdAsync()
    {
        if (ControllerConnected && _client.Connected)
            await _client.SocketIoClient.EmitAsync(Command, Controller.Port, Feedhold);
    }

    public async Task CyclestartAsync()
    {
        if (ControllerConnected && _client.Connected)
            await _client.SocketIoClient.EmitAsync(Command, Controller.Port, Cyclestart);
    }

    internal void Clear()
    {
        Controller = null;
        ControllerState = null;
        ControllerSettings = null;
    }

}