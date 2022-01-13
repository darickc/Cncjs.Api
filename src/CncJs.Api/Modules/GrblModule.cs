using System.Text.Json;
using CncJs.Api.Models;

namespace CncJs.Api.Modules;

public class GrblModule
{
    private readonly CncJsClient _client;

    private const string Settings = "Grbl:settings";
    private const string State    = "Grbl:state";

    public event EventHandler<GrblSettings> OnSettings;
    public event EventHandler<State> OnState;

    internal GrblModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(Settings, response => OnSettings?.Invoke(this, GetSettings(response.GetValue())));
        _client.SocketIoClient.On(State, response => OnState?.Invoke(this, response.GetValue<State>()));
    }
    

    public static GrblSettings GetSettings(JsonElement json)
    {
        var settings = new GrblSettings();

        if (json.TryGetProperty("version", out var value))
        {
            settings.Version = value.GetString();
        }
        if (json.TryGetProperty("settings", out var item))
        {
            foreach (var t in item.EnumerateObject())
            {
                settings.Settings.Add(t.Name, t.Value.GetString());
            }
        }

        return settings;
    }
}