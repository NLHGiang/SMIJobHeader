using SMIJobHeader.Extensions;

namespace SMIJobHeader.Utils;

public static class DictionaryExtension
{
    public static void AddOrUpdate(this Dictionary<string, string> dic, string key, string? value)
    {
        if (dic == null) return;
        if (!dic.ContainsKey(key))
            dic.Add(key, value);
        else
            dic[key] = value;
    }

    public static void UpdateWhenExist(this Dictionary<string, string> dic, string key, string? value)
    {
        if (dic == null) return;
        if (dic.ContainsKey(key)) dic[key] = value;
    }

    public static void AddOrUpdate(this Dictionary<string, string> dic, string key, string? value, string subKey)
    {
        if (dic == null) return;
        dic.RemoveKeyContain(subKey);
        if (!dic.ContainsKey(key))
            dic.Add(key, value);
        else
            dic[key] = value;
    }


    public static string GetValue(this Dictionary<string, string> dic, string? key)
    {
        if (dic == null || !dic.Any() || key.IsNullOrEmpty()) return string.Empty;
        if (!dic.ContainsKey(key)) return string.Empty;
        return dic.GetValueOrDefault(key);
    }

    public static void RemoveKey(this Dictionary<string, string> dic, string? key)
    {
        if (dic == null || !dic.Any() || key.IsNullOrEmpty()) return;
        if (!dic.ContainsKey(key)) return;
        dic.Remove(key);
    }

    public static void RemoveKeyContain(this Dictionary<string, string> dic, string? key)
    {
        if (dic == null || !dic.Any() || key.IsNullOrEmpty()) return;
        foreach (var item in dic)
            if (item.Key.Contains(key))
                dic.Remove(item.Key);
    }
}