using System.Text;
using System.Text.RegularExpressions;
using SMIJobHeader.Utils;

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

    public static bool IsEquals(this string str, string value)
    {
        return str.EmptyNull().Equals(value);
    }

    public static string EmptyNull(this string value)
    {
        return value ?? "";
    }

    public static string MTrim(this object value)
    {
        if (value == null) return string.Empty;
        return value.ToString().Trim();
    }

    public static int ToInt(this string value, int valueDefault)
    {
        if (value == null) return valueDefault;

        _ = int.TryParse(value, out var outValue);
        return outValue;
    }

    public static int? ToInt(this string value)
    {
        if (value == null) return null;

        _ = int.TryParse(value, out var outValue);
        return outValue;
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

    public static string ConvertToString(this object value)
    {
        if (value == null) return string.Empty;

        return value.ToString();
    }

    public static string RemoveNumber(this string value)
    {
        if (value.IsNullOrEmpty()) return string.Empty;
        return Regex.Replace(value, @"[\d-]", string.Empty).Trim();
    }

    public static string RemoveAllZero(this string value, char specter, char charactorWillBeRemove)
    {
        var phanle = value.SpliptData(specter, 1);
        if (phanle.IsNullOrEmpty()) return value;
        if (phanle.ToInt(0) > 0) return value.RemoveCharactorEndString(charactorWillBeRemove);
        return value.RemoveCharactorEndString(charactorWillBeRemove).ReplaceAndTrim(specter.ToString(), "");
    }

    public static string SpliptData(this object value, char separator, int indexof)
    {
        var valueOfTex = value.ConvertToString();
        string[] array = valueOfTex.Split(separator);
        if (array.Length > indexof) return array[indexof];

        return string.Empty;
    }

    public static string RemoveCharactorEndString(this string value, char charactorWillBeRemove)
    {
        if (value.IsNullOrEmpty()) return value;

        var valueAfterRemove = value.Reverse().SkipWhile(i => i == charactorWillBeRemove).Reverse();
        return new string(valueAfterRemove.ToArray());
    }
}