namespace CncJs.Api.Models;

public class ControllerTypes
{
    public const string Grbl = "Grbl";
    public const string Marlin = "Marlin";
    public const string Smoothie = "Smoothie";
    public const string TinyG = "TinyG";

    public static string[] AsList { get; set; } = { Grbl, Marlin, Smoothie, TinyG };
}