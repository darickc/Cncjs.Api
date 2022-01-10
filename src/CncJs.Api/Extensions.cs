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
}