using System.Globalization;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using SMIJobHeader.Constants;
using SMIJobHeader.Utils;

namespace SMIJobHeader.Extensions;

public static class ObjectExtensions
{
    public static bool ToBoolean(this object value)
    {
        if (value == null) return false;
        return value.ToString().EmptyNull().ToLower() switch
        {
            "true" => true,
            "t" => true,
            "1" => true,
            "0" => false,
            "false" => false,
            "f" => false,
            "" => false,
            _ => throw new InvalidCastException("You can't cast a weird value to a bool!")
        };
    }

    public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
    {
        if (second == null || first == null) return;
        foreach (var item in second)
            if (!first.ContainsKey(item.Key))
                first.Add(item.Key, item.Value);
    }

    public static void CopyPropertiesFrom(this object self, object parent, bool onlySelfFieldNull = true,
        bool notIncludeId = true)
    {
        var fromProperties = parent.GetType().GetProperties();
        var toProperties = self.GetType().GetProperties();

        foreach (var fromProperty in fromProperties)
        {
            if ((notIncludeId && fromProperty.Name.ToLower() == "id")
                || !fromProperty.CanRead)
                continue;

            var toProperty = toProperties.Where(p => p.Name == fromProperty.Name).FirstOrDefault();
            if (toProperty == null) continue;

            var toValue = toProperty.GetValue(self);
            if ((onlySelfFieldNull && toValue != null) || !toProperty.CanWrite) continue;

            var parentValue = fromProperty.GetValue(parent);
            if (parentValue != null) toProperty.SetValue(self, parentValue);
        }
    }

    public static List<List<Guid>> ChunkBy(this List<Guid> source, int chunkSize)
    {
        if (source == null || !source.Any()) return new List<List<Guid>>();
        return source
            .Distinct()
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }

    public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
    {
        if (source == null || !source.Any()) return new List<List<T>>();
        return source
            .Distinct()
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }


    public static T DeserializeObject<T>(this string jsonStr)
    {
        if (jsonStr.IsNullOrEmpty()) return default;
        return JsonConvert.DeserializeObject<T>(jsonStr);
    }

    public static string SerializeObjectToString(this object value)
    {
        if (value == null) return string.Empty;

        return JsonConvert.SerializeObject(value);
    }

    private static string GetValueOfList(List<string> array, int index)
    {
        try
        {
            return array[index];
        }
        catch
        {
            return string.Empty;
        }
    }

    public static object GetValue(this JsonObject jsonObject, string fileName, string defaultValue, Type destDataType,
        string valueReplace)
    {
        object result = null;
        if (jsonObject == null) return result;

        if (jsonObject[fileName] == null && defaultValue.IsNullOrEmpty()) return result;

        if (jsonObject[fileName] == null) jsonObject[fileName] = defaultValue;
        return jsonObject[fileName].ConvertData(destDataType, valueReplace);
    }

    private static object ConvertData(this JsonNode jsonNote, Type destDataType, string valueReplace)
    {
        if (jsonNote == null) return null;
        try
        {
            var propertyType = Nullable.GetUnderlyingType(destDataType) ?? destDataType;
            var dataType = propertyType.GetPropertyName(out var isNullable);
            return dataType switch
            {
                DATATYPE.DOUBLE => jsonNote.GetValueDouble(isNullable),
                DATATYPE.DECIMAL => jsonNote.GetValueDecimal(isNullable),
                DATATYPE.INT32 => jsonNote.GetValueInt(isNullable),
                DATATYPE.BOOLEAN => jsonNote.GetValueBool(isNullable),
                DATATYPE.GUID => jsonNote.GetValueGuid(isNullable),
                DATATYPE.DATETIME => jsonNote.GetValueDateTime(isNullable),
                _ => jsonNote.GetValueString(valueReplace)
            };
        }
        catch
        {
            return jsonNote.ConvertDataToString(valueReplace);
        }
    }

    private static object ConvertDataToString(this JsonNode jsonNote, string valueReplace)
    {
        if (jsonNote == null) return null;
        try
        {
            return jsonNote.GetValueString(valueReplace);
        }
        catch
        {
            return null;
        }
    }

    private static JsonNode RetrySetDataString(object? value)
    {
        if (value == null) return null;
        try
        {
            return (string)value;
        }
        catch
        {
            return null;
        }
    }

    public static string FomatMoney(this object rawValue, CultureInfo culture, string format, bool isRemoveZero = false)
    {
        if (rawValue == null || format.IsNullOrEmpty()) return string.Empty;
        var result = string.Format(culture, format, rawValue);
        if (!isRemoveZero) return result;
        var specter = GetSpecter(culture);
        return result.RemoveAllZero(specter, '0');
    }


    private static char GetSpecter(CultureInfo culture)
    {
        var result = ',';
        if (culture == null) return result;
        result = CommonConstants.DefaultCulture.Equals(culture.Name) ? ',' : '.';
        return result;
    }

    public static int? GetValueInt(this JsonNode jsonNote, bool isNullable)
    {
        try
        {
            if (jsonNote == null)
            {
                int? valueDefault = isNullable ? null : 0;
                return valueDefault;
            }

            return jsonNote.ToString().ToInt();
        }
        catch
        {
            return isNullable ? null : 0;
        }
    }

    public static double? GetValueDouble(this object value, bool isNullable)
    {
        try
        {
            if (value == null)
            {
                int? valueDefault = isNullable ? null : 0;
                return valueDefault;
            }

            return double.Parse(double.Parse(value.ToString(), CultureInfo.InvariantCulture).ToString());
        }
        catch
        {
            return isNullable ? null : 0;
        }
    }

    public static decimal? GetValueDecimal(this object value, bool isNullable)
    {
        try
        {
            if (value == null)
            {
                int? valueDefault = isNullable ? null : 0;
                return valueDefault;
            }

            return decimal.Parse(double.Parse(value.ToString(), CultureInfo.InvariantCulture).ToString());
        }
        catch
        {
            return isNullable ? null : 0;
        }
    }

    public static decimal? GetValueDecimal(this JsonNode jsonNote, bool isNullable)
    {
        try
        {
            if (jsonNote == null)
            {
                int? valueDefault = isNullable ? null : 0;
                return valueDefault;
            }

            return decimal.Parse(jsonNote.ToString(), CultureInfo.InvariantCulture);
        }
        catch
        {
            return isNullable ? null : 0;
        }
    }

    public static double? GetValueDouble(this JsonNode jsonNote, bool isNullable)
    {
        try
        {
            if (jsonNote == null)
            {
                int? valueDefault = isNullable ? null : 0;
                return valueDefault;
            }

            return double.Parse(jsonNote.ToString(), CultureInfo.InvariantCulture);
        }
        catch
        {
            return isNullable ? null : 0;
        }
    }

    public static bool GetValueBool(this JsonNode jsonNote, bool isNullable)
    {
        try
        {
            if (jsonNote == null) return false;

            return jsonNote.ToString().ToBoolean();
        }
        catch
        {
            return false;
        }
    }

    public static Guid? GetValueGuid(this JsonNode jsonNote, bool isNullable)
    {
        try
        {
            if (jsonNote == null) return null;

            return jsonNote.GetValue<Guid?>();
        }
        catch
        {
            return null;
        }
    }

    public static DateTime? GetValueDateTime(this JsonNode jsonNote, bool isNullable)
    {
        try
        {
            if (jsonNote == null) return null;

            return jsonNote.GetValue<DateTime?>();
        }
        catch
        {
            return null;
        }
    }

    public static string? GetValueString(this JsonNode jsonNote, string valueReplace)
    {
        try
        {
            if (jsonNote == null) return null;
            var value = jsonNote.ToString();
            if (value.IsNullOrEmpty() || valueReplace.IsNullOrEmpty()) return value.MTrim();
            var oldValue = valueReplace.SpliptData('$', 0);
            var newValue = valueReplace.SpliptData('$', 1);
            if (!value.MTrim().IsEquals(oldValue)) return value.MTrim();

            return value.Replace(oldValue, newValue).MTrim();
        }
        catch
        {
            return null;
        }
    }

    public static decimal ToDecimal(this object value, decimal valueDefault)
    {
        if (value == null) return valueDefault;

        _ = decimal.TryParse(value.ToString(), out var outValue);
        return outValue;
    }

    public static string MToString(this DateTime? value, string fomat)
    {
        if (!value.HasValue) return null;
        return value.Value.ToString(fomat);
    }


    private static object? RoundNumber(this object? money, Type type, int decimalNumber)
    {
        if (money == null) return money;
        var dataType = type.GetPropertyName(out _);
        var destData = dataType switch
        {
            DATATYPE.DOUBLE => Math.Round((double)money, decimalNumber, MidpointRounding.AwayFromZero),
            DATATYPE.DECIMAL => Math.Round((decimal)money, decimalNumber, MidpointRounding.AwayFromZero),
            _ => money
        };
        return destData;
    }

    public static bool IsDefaultValue(this Type destDataType, object valueOfObjectTaget)
    {
        if (valueOfObjectTaget == null) return true;
        try
        {
            var propertyType = Nullable.GetUnderlyingType(destDataType) ?? destDataType;
            var dataType = propertyType.GetPropertyName(out var isNullable);
            return dataType switch
            {
                DATATYPE.DOUBLE => Convert.ToDouble(valueOfObjectTaget) == 0,
                DATATYPE.DECIMAL => Convert.ToDecimal(valueOfObjectTaget) == 0,
                DATATYPE.INT32 => Convert.ToInt32(valueOfObjectTaget) == 0,
                DATATYPE.BOOLEAN => Convert.ToBoolean(valueOfObjectTaget) == false,
                _ => false
            };
        }
        catch
        {
            return true;
        }
    }
}