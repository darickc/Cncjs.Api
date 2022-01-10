namespace Cncjs.Api.Models;

public class State
{
    public Status Status { get; set; }
    public Parserstate Parserstate { get; set; }
    public Units Units => Parserstate?.Modal?.Units switch{ "G20" => Units.Imperial, _ =>Units.Metric};
}