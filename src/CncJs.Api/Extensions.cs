using Cncjs.Api.Models;

namespace Cncjs.Api;

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
    public static int? ToDouble(this string value)
    {
        if (int.TryParse(value, out var result))
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
}