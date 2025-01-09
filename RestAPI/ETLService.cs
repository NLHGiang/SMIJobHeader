using SMIJobXml.Extensions;
using SMIJobXml.Model.Job;
using SMIJobXml.RestAPI.Interface;

namespace SMIJobXml.RestAPI
{
    public class ETLService : IETLService
    {
        private static string ApiBaseUrl = "http://127.0.0.1:44314/api/";
        private static readonly string UrlRecurringSendEmail = "app/tenant-company/recurring-send-email";
        private readonly RestClient _restClient;
        public ETLService(string apiBaseUrl)
        {
            ApiBaseUrl = apiBaseUrl;
            _restClient = new RestClient(ApiBaseUrl);
        }

        public ETLService(string apiBaseUrl, string authenTokenKey, string authenToken) : this(apiBaseUrl)
        {
            _restClient?.DefaultRequestHeaders.Add(authenTokenKey, authenToken);
        }

        public async Task<T> Synchronized<T>(BatchJob modelDto)
        {
            var response = await this._restClient.MPostHttpContentAsync<BatchJob>(UrlRecurringSendEmail, modelDto);
            return await response.ReadAsAsync<T>();
        }
    }
}
