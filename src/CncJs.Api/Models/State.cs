namespace Cncjs.Api.Models;

public class State
{
    public Status Status { get; set; }
    public Parserstate Parserstate { get; set; }
    public string Units => Parserstate?.Modal?.Units switch{ "G21"=> "MM", "G20" => "IN", _ =>""};
}