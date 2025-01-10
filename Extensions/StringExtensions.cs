using System.Text;
using Newtonsoft.Json;

namespace SMIJobHeader.Extensions;

public static class StringExtensions
{
    public static bool IsNotNullOrEmpty(this string value)
    {
        return !string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }


    public static int ToInt(this string value, int valueDefault)
    {
        if (value == null) return valueDefault;

        _ = int.TryParse(value, out var outValue);
        return outValue;
    }

    public static T DeserializeObject<T>(this string jsonStr)
    {
        try
        {
            if (string.IsNullOrEmpty(jsonStr)) return default;
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
        catch
        {
            return default;
        }
    }

    public static string SerializeToJson(this object value)
    {
        if (value == null) return string.Empty;

        return JsonConvert.SerializeObject(value);
    }

    public static byte[] ConvertToBytes(this string value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        return Encoding.UTF8.GetBytes(value);
    }

    public static string ConvertBytesToString(this byte[] value)
    {
        if (value == null) return null;
        return Encoding.UTF8.GetString(value);
    }
}