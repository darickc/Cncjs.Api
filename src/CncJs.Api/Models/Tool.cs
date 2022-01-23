namespace CncJs.Api.Models;

public class Tool
{
    public int Number { get; set; }
    public string Description { get; set; }

    public Tool(int number, string description)
    {
        Number = number;
        Description = description;
    }

    public override string ToString()
    {
        var temp = !string.IsNullOrEmpty(Description) ? " - " : "";
        return $"{Number}{temp}{Description}";
    }
}