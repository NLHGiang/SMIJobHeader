using SMIJobHeader.Extensions;
using System.Net.Http.Headers;

namespace SMIJobHeader.RestAPI;

public class RestClient : HttpClient
{
    public RestClient(string baseApiUri)
    {
        if (string.IsNullOrEmpty(baseApiUri)) throw new ArgumentNullException();

        BaseAddress = new Uri(baseApiUri);
        DefaultRequestHeaders.Accept.Clear();
        DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public RestClient(string baseApiUri, string mediaType)
    {
        if (string.IsNullOrEmpty(baseApiUri)) throw new ArgumentNullException();

        BaseAddress = new Uri(baseApiUri);
        DefaultRequestHeaders.Accept.Clear();
        DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
    }

    public T Get<T>(string requestUri)
        where T : class
    {
        var response = GetAsync(requestUri).Result;

        response.CustomEnsureSuccessStatusCode();

        var resultData = response.Content.ReadAsAsync<T>();
        return resultData.Result;
    }

    public async Task<T> GetAsync<T>(string requestUri)
        where T : class
    {
        var response = await GetAsync(requestUri);

        response.CustomEnsureSuccessStatusCode();

        var resultData = await response.Content.ReadAsAsync<T>();

        return resultData;
    }

    public string GetJson(string requestUri)
    {
        var response = GetAsync(requestUri).Result;
        response.CustomEnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }

    public async Task<HttpContent> MPostHttpContentAsync<T>(string requestUri, T t)
        where T : class
    {
        var response = await this.MPostAsJsonAsync(requestUri, t);
        response.CustomEnsureSuccessStatusCode();
        return response.Content;
    }

    public string Post<T>(string requestUri, T t)
        where T : class
    {
        var response = this.MPostAsJsonAsync(requestUri, t).Result;
        response.CustomEnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }

    public U Post<T, U>(string requestUri, T t)
        where T : class
        where U : class
    {
        var response = this.PostAsJsonAsync(requestUri, t).Result;

        // Throw exception if HTTP status code is not Success (2xx)
        response.CustomEnsureSuccessStatusCode();

        return response.Content.ReadAsAsync<U>().Result;
    }

    public async Task<string> MPostAsync<T>(string requestUri, T t)
        where T : class
    {
        var response = await this.MPostAsJsonAsync(requestUri, t);
        response.CustomEnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public HttpContent Post(string requestUri, HttpContent? content)
    {
        var response = PostAsync(requestUri, content).Result;
        response.CustomEnsureSuccessStatusCode();
        return response.Content;
    }

    public async Task<T> MPostAsync<T>(string requestUri,
        IEnumerable<KeyValuePair<string?, string?>> nameValueCollection)
        where T : class
    {
        var response = await PostAsync(requestUri, new FormUrlEncodedContent(nameValueCollection));
        response.CustomEnsureSuccessStatusCode();
        var resultData = await response.Content.ReadAsAsync<T>();
        return resultData;
    }

    public bool Delete(string requestUri)
    {
        var response = DeleteAsync(requestUri).Result;

        //// Throw exception if HTTP status code is not Success (2xx)
        response.CustomEnsureSuccessStatusCode();

        return response.IsSuccessStatusCode;
    }
}