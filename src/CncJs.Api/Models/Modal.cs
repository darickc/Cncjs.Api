namespace Cncjs.Api.Models;

public class Modal
{
    public string Motion { get; set; }
    public string Wcs { get; set; }
    public string Plane { get; set; }
    public string Units { get; set; }
    public string Distance { get; set; }
    public string Feedrate { get; set; }
    public string Spindle { get; set; }
    public string Coolant { get; set; }

    public string Workspace => Wcs switch
    {
        "G54" => "P1", "G55" => "P2", "G56" => "P3", "G57" => "P4", "G58" => "P5", "G59" => "P6"
    };
}