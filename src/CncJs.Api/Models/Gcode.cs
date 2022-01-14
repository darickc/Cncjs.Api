namespace CncJs.Api.Models;

public class Gcode
{
    public string FileName { get; set; }
    public string Code { get; set; }
    public Context Context { get; set; }

    public Gcode(string fileName, string code)
    {
        FileName = fileName;
        Code = code;
    }
}