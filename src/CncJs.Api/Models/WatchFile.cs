namespace CncJs.Api.Models;

public class WatchFile
{
    public string Path { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public WatchFileType FileType => Type switch
    {
        "f" => WatchFileType.File,
        "d" => WatchFileType.Directory,
        _   => throw new ArgumentOutOfRangeException()
    };
    public int Size { get; set; }
    public DateTime Atime { get; set; }
    public DateTime Mtime { get; set; }
    public DateTime Ctime { get; set; }

    public bool Expanded { get; set; }
    public bool Loading { get; set; }
    public bool Selected { get; set; }

    public WatchFile[] Files { get; set; }
}