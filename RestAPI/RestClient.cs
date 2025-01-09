using SMIJobXml.Extensions;
using System.Net.Http.Headers;

namespace SMIJobXml.RestAPI
{
    public class RestClient : HttpClient
    {
        public RestClient(string baseApiUri)
        {
            if (string.IsNullOrEmpty(baseApiUri))
            {
                throw new ArgumentNullException();
            }

            this.BaseAddress = new Uri(baseApiUri);
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public RestClient(string baseApiUri, string mediaType)
        {
            if (string.IsNullOrEmpty(baseApiUri))
            {
                throw new ArgumentNullException();
            }

            this.BaseAddress = new Uri(baseApiUri);
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }

        public T Get<T>(string requestUri)
            where T : class
        {
            HttpResponseMessage response = this.GetAsync(requestUri).Result;

            response.CustomEnsureSuccessStatusCode();

            var resultData = response.Content.ReadAsAsync<T>();
            return resultData.Result;
        }

        public async Task<T> GetAsync<T>(string requestUri)
            where T : class
        {
            HttpResponseMessage response = await this.GetAsync(requestUri);

            response.CustomEnsureSuccessStatusCode();

            var resultData = await response.Content.ReadAsAsync<T>();

            return resultData;
        }

        public string GetJson(string requestUri)
        {
            HttpResponseMessage response = this.GetAsync(requestUri).Result;
            response.CustomEnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<HttpContent> MPostHttpContentAsync<T>(string requestUri, T t)
            where T : class
        {
            HttpResponseMessage response = await this.MPostAsJsonAsync<T>(requestUri, t);
            response.CustomEnsureSuccessStatusCode();
            return response.Content;
        }

        public string Post<T>(string requestUri, T t)
            where T : class
        {
            HttpResponseMessage response = this.MPostAsJsonAsync<T>(requestUri, t).Result;
            response.CustomEnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        public U Post<T, U>(string requestUri, T t)
            where T : class
            where U : class
        {

            HttpResponseMessage response = this.PostAsJsonAsync<T>(requestUri, t).Result;

            // Throw exception if HTTP status code is not Success (2xx)
            response.CustomEnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<U>().Result;

        }

        public async Task<string> MPostAsync<T>(string requestUri, T t)
            where T : class
        {
            HttpResponseMessage response = await this.MPostAsJsonAsync<T>(requestUri, t);
            response.CustomEnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public HttpContent Post(string requestUri, HttpContent? content)
        {
            HttpResponseMessage response = this.PostAsync(requestUri, content).Result;
            response.CustomEnsureSuccessStatusCode();
            return response.Content;
        }

        public async Task<T> MPostAsync<T>(string requestUri, IEnumerable<KeyValuePair<string?, string?>> nameValueCollection)
            where T : class
        {
            HttpResponseMessage response = await this.PostAsync(requestUri, new FormUrlEncodedContent(nameValueCollection));
            response.CustomEnsureSuccessStatusCode();
            var resultData = await response.Content.ReadAsAsync<T>();
            return resultData;
        }

        public bool Delete(string requestUri)
        {
            HttpResponseMessage response = base.DeleteAsync(requestUri).Result;

            //// Throw exception if HTTP status code is not Success (2xx)
            response.CustomEnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }
    }
}
