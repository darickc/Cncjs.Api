using System.Text.Json;
using Cncjs.Api.Models;

namespace Cncjs.Api;

public class Grbl
{
    private readonly CncJsSocketIo _client;

    private const string Settings = "Grbl:settings";
    private const string State    = "Grbl:state";

    public Action<GrblSettings> OnSettings { get; set; }
    public Action<State> OnState { get; set; }

    internal Grbl(CncJsSocketIo client)
    {
        _client = client;
        _client.On(Settings, response => OnSettings?.Invoke(GetSettings(response.GetValue())));
        _client.On(State, response => response.GetValue<State>());
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