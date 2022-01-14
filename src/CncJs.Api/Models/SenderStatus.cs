namespace CncJs.Api.Models;

public class SenderStatus
{
    public int Sp { get; set; }
    public bool Hold { get; set; }
    //public string HoldReason { get; set; }
    public string Name { get; set; }
    public Context Context { get; set; }
    public long Size { get; set; }
    public long Total { get; set; }
    public long Sent { get; set; }
    public long Received { get; set; }
    public long StartTime { get; set; }
    public long FinishTime { get; set; }
    public double ElapsedTime { get; set; }
    public double RemainingTime { get; set; }

    public TimeSpan Elapsed => TimeSpan.FromMilliseconds(ElapsedTime);
    public TimeSpan Remaining => TimeSpan.FromMilliseconds(RemainingTime);
}