using Newtonsoft.Json;
using System.Text;

namespace SMIJobXml.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<HttpResponseMessage> MPostAsJsonAsync<T>(this HttpClient client, string? requestUri, T value)
        {
            string serializeObject = JsonConvert.SerializeObject(value);
            var stringContents = new StringContent(serializeObject, Encoding.UTF8, "application/json");
            return await client.PostAsync(requestUri, stringContents);

        }
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            var strContents = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(strContents);
        }
    }
}
