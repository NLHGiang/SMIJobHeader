namespace SMIJobHeader.Utils;

public static class CookiesExtension
{
    public static string UpdateCookies(string? oldCookies, string? newCookies)
    {
        var cookies = new Dictionary<string, string>();

        ProcessCookies(oldCookies, cookies);
        ProcessCookies(newCookies, cookies);

        return string.Join("; ", cookies.Select(cookie => $"{cookie.Key}={cookie.Value}"));
    }

    private static void ProcessCookies(string? cookieHeader, Dictionary<string, string> cookies)
    {
        if (string.IsNullOrWhiteSpace(cookieHeader)) return;

        foreach (var pair in cookieHeader.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var keyValue = pair.Split(new[] { '=' }, 2);
            if (keyValue.Length == 2) cookies[keyValue[0].Trim()] = keyValue[1].Trim();
        }
    }
}