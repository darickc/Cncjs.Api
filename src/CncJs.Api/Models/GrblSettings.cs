namespace Cncjs.Api.Models;

public class GrblSettings : Settings
{
    public string Version { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();
}