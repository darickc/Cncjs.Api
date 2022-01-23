using System.Text.RegularExpressions;

namespace CncJs.Api.Models;

public class Gcode
{
    public string FileName { get; set; }
    public string Code { get; set; }
    public List<Tool> Tools { get; set; } = new();
    public int CurrentToolIndex { get; set; }
    public List<string> Lines { get; set; }
    public Tool CurrentTool => Tools.Count > CurrentToolIndex ? Tools[CurrentToolIndex] : null;


    private const string ToolExpression = @"T(?<tool_number>\d+)\s*(\((?<tool_description>.*)\))?";

    public Gcode(string fileName, string code)
    {
        FileName = fileName;
        Code = code;
        Lines = code.Split("\n").ToList();
        if (Lines.Any())
        {
            foreach (var line in Lines)
            {
                foreach (Match match in Regex.Matches(line, ToolExpression, RegexOptions.IgnoreCase))
                {
                    if (int.TryParse(match.Groups["tool_number"].Value, out var number))
                    {
                        var description = match.Groups.ContainsKey("tool_description")
                            ? match.Groups["tool_description"].Value
                            : "";
                        Tools.Add(new Tool(number, description));
                    }
                }
            }
        }
    }
}