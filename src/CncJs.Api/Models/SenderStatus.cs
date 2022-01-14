namespace CncJs.Api.Models;

public class SenderStatus
{
    public int Sp { get; set; }
    public bool Hold { get; set; }
    public object HoldReason { get; set; }
    public string Name { get; set; }
    public Context Context { get; set; }
    public int Size { get; set; }
    public int Total { get; set; }
    public int Sent { get; set; }
    public int Received { get; set; }
    public int StartTime { get; set; }
    public int FinishTime { get; set; }
    public int ElapsedTime { get; set; }
    public int RemainingTime { get; set; }
}