namespace Cncjs.Api.Models;

public class StartupModel
{
    public List<string> LoadedControllers { get; set; }
    public List<int> Baudrates { get; set; }
    public List<string> Ports { get; set; }
}