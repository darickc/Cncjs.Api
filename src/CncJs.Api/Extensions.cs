using CncJs.Api.Models;

namespace CncJs.Api;

public static class Extensions
{
    public static int? ToInt(this string value)
    {
        if (int.TryParse(value, out var result))
        {
            return result;
        }

        return null;
    }
    public static double? ToDouble(this string value)
    {
        if (double.TryParse(value, out var result))
        {
            return result;
        }

        return null;
    }

    public static double ToMetric(this double inches)
    {
        return inches * 25.4;
    }

    public static double ToImperial(this double mm)
    {
        return mm / 25.4;
    }

    public static string AsString(this Units units) => units switch
    {
        Units.Imperial => "IN",
        _ => "MM"
    };

    public static string AsString(this TimeSpan span)
    {
        var temp = span.Days > 0 ? "d'.'" : "";
        temp += span.Hours > 0 || span.Days > 0 ? "h':'" : "";
        temp += "mm':'ss";
        return span.ToString(temp);
    }
}