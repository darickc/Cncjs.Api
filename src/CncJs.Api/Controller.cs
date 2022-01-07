using Cncjs.Api.Models;
using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class Controller
{
    private readonly CncJsSocketIo _client;
    private const    string        Settings = "controller:settings";
    private const    string        State    = "controller:state";
    public Action<GrblSettings> OnSettings { get; set; }
    public Action OnState { get; set; }
    internal Controller(CncJsSocketIo client)
    {
        _client = client;
        _client.On(Settings, OnSettingsEvent);
        _client.On(State, OnStateEvent);
    }
    private void OnSettingsEvent(SocketIOResponse obj)
    {
        var controllerType = obj.GetValue<string>();
        var settings = new GrblSettings();

        if (obj.GetValue(1).TryGetProperty("version", out var value))
        {
            settings.Version = value.GetString();
        }
        if (obj.GetValue(1).TryGetProperty("settings", out var item))
        {
            foreach (var t in item.EnumerateObject())
            {
                settings.Settings.Add(t.Name, t.Value.GetString());
            }
        }

        OnSettings?.Invoke(settings);
    }
    private void OnStateEvent(SocketIOResponse obj)
    {
        OnState?.Invoke();
    }

}