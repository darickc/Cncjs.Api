using Cncjs.Api.Models;
using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class Controller
{
    private readonly CncJsSocketIo _client;
    private const    string        Settings = "controller:settings";
    private const    string        State    = "controller:state";
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

}