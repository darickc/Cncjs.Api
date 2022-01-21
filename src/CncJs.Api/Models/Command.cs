namespace CncJs.Api.Models;

public class Command
{
    public string Id { get; set; }
    public long Mtime { get; set; }
    public bool Enabled { get; set; }
    public string Title { get; set; }
    public string Commands { get; set; }
}