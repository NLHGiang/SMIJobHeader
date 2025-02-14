using SMIJobHeader.Model.CrawlData;
using System.Net.Http.Headers;

namespace SMIJobHeader.RestAPI;

public class CrawlApiService
{
    private const string BasicAuthenToken = "Bearer";
    private const string uriReceptionMessage = "/api/v1/crawl/reception-message";

    private readonly RestClient _restClient;

    public CrawlApiService()
    {
    }

    public CrawlApiService(string baseAPI)
    {
        _restClient = new RestClient(baseAPI);
    }

    public CrawlApiService(string baseAPI, string authenToken)
        : this(baseAPI)
    {
        if (_restClient != null)
            _restClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(BasicAuthenToken, authenToken);
    }


    public void PushResultCrawl(ResponseDto request)
    {
        if (request == null) return;
        _restClient.Post<ResponseDto, ApiResponse>(uriReceptionMessage, request);
    }

    public async Task CrawlEInvoice(CrawlEInvoice request, string uri)
    {
        if (request == null) return;
        await _restClient.MPostAsync<CrawlEInvoice>(uri, request);
    }
}