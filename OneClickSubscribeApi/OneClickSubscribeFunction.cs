using System.Net;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;
using OneClickSubscribeApi.Options;
using Microsoft.Azure.Functions.Worker.Http;
using OneClickSubscribeApi.Domain.Services;

namespace OneClickSubscribeApi;

public class OneClickSubscribeFunction
{
    private readonly ISubscriptionProcessingService _subscriptionProcessingService;
    private readonly ApplicationOptions _options;

    public OneClickSubscribeFunction(IOptions<ApplicationOptions> options, ISubscriptionProcessingService subscriptionProcessingService)
    {
        _subscriptionProcessingService = subscriptionProcessingService;
        _options = options.Value;
    }

    private static string EmailQueryParam => "email";
    private static string FirstNameQueryParam => "firstname";
    private static string LastNameQueryParam => "lastname";
    private static string TypeQueryParam => "type";

    [Function(nameof(OneClickSubscribeFunction))]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestData request)
    {
        var queryValues = GetQueryValues(request.Url.Query);

        queryValues.TryGetValue(EmailQueryParam, out var email);
        queryValues.TryGetValue(FirstNameQueryParam, out var firstname);
        queryValues.TryGetValue(LastNameQueryParam, out var lastname);
        queryValues.TryGetValue(TypeQueryParam, out var type);

        return await _subscriptionProcessingService.TryProcessSubscriberAsync((email, firstname, lastname, type))
                ? CreateRedirectResponse(request, _options.RedirectToSubscribed)
                : CreateRedirectResponse(request, _options.RedirectToForm);
    }

    private static Dictionary<string, string> GetQueryValues(string query)
    {
        var queryValues = new Dictionary<string, string>();

        query = HttpUtility.UrlDecode(query);

        var queryParts = query.Replace("?", string.Empty)
            .Split('&')
            .Select(q => q.Split('=')).ToList();

        if(queryParts.Any(q => q.Length == 2))
            queryValues = queryParts
                .Where(qp => qp.Length == 2)
                .ToDictionary(k => k[0], v => v[1]);

        return queryValues;
    }

    private static HttpResponseData CreateRedirectResponse(HttpRequestData request, string location)
    {
        var response = request.CreateResponse(HttpStatusCode.Redirect);

        response.Headers.Add("Location", location);

        return response;
    }
}