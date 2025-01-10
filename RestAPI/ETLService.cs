using SMIJobHeader.Extensions;
using SMIJobHeader.Model.Job;
using SMIJobHeader.RestAPI.Interface;

namespace SMIJobHeader.RestAPI;

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
        var response = await _restClient.MPostHttpContentAsync(UrlRecurringSendEmail, modelDto);
        return await response.ReadAsAsync<T>();
    }
}