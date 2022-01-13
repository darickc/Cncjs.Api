namespace CncJs.Api.Models;

public class Status
{
    public string ActiveState { get; set; }
    public Position Mpos { get; set; }
    public Position Wpos { get; set; }
    public List<int> Ov { get; set; }
    public int SubState { get; set; }
    public Position Wco { get; set; }
    public int Feedrate { get; set; }
    public int Spindle { get; set; }
}