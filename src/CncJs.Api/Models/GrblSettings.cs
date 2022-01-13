namespace CncJs.Api.Models;

public class GrblSettings : Settings
{
    public string Version { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();

    public override string GetSetting(string key)
    {
        if (Settings.TryGetValue(key, out var value))
        {
            return value;
        }

        return null;
    }
}