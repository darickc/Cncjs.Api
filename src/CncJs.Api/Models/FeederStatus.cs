namespace CncJs.Api.Models;

public class FeederStatus
{
    public bool Hold { get; set; }
    public object HoldReason { get; set; }
    public int Queue { get; set; }
    public bool Pending { get; set; }
    public bool Changed { get; set; }
}