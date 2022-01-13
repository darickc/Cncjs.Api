namespace CncJs.Api.Models;

public class WatchFile
{
    public string Name { get; set; }
    public string Type { get; set; }
    public WatchFileType FileType => Type switch
    {
        "f" => WatchFileType.File,
        "d" => WatchFileType.Directory,
        _   => throw new ArgumentOutOfRangeException()
    };
    public int Size { get; set; }
}