using System.Text;
using Newtonsoft.Json;

namespace SMIJobHeader.Extensions;

public static class HttpContentExtensions
{
    public static async Task<HttpResponseMessage> MPostAsJsonAsync<T>(this HttpClient client, string? requestUri,
        T value)
    {
        var serializeObject = JsonConvert.SerializeObject(value);
        var stringContents = new StringContent(serializeObject, Encoding.UTF8, "application/json");
        return await client.PostAsync(requestUri, stringContents);
    }

    public static async Task<T> ReadAsAsync<T>(this HttpContent content)
    {
        var strContents = await content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(strContents);
    }
}