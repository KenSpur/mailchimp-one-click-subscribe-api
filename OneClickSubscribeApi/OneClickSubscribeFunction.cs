using System.Net;
using System.Net.Mail;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;
using OneClickSubscribeApi.Options;
using OneClickSubscribeApi.Services;
using Microsoft.Azure.Functions.Worker.Http;

namespace OneClickSubscribeApi;

public class OneClickSubscribeFunction
{
    private readonly IStorageService _storageService;
    private readonly ApplicationOptions _options;

    public OneClickSubscribeFunction(IOptions<ApplicationOptions> options, IStorageService storageService)
    {
        _storageService = storageService;
        _options = options.Value;
    }

    private static string EmailQueryParam => "email";
    private static string TypeQueryParam => "type";

    [Function(nameof(OneClickSubscribeFunction))]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestData request)
    {
        var queryValues = GetQueryValues(request.Url.Query);

        if (!queryValues.TryGetValue(EmailQueryParam, out var email) || !IsValidEmail(email))
            return CreateRedirectResponse(request, _options.RedirectToForm);

        queryValues.TryGetValue(TypeQueryParam, out var type);

        await _storageService.AddSubscriberAsync(email, type);

        return CreateRedirectResponse(request, _options.RedirectToSubscribed);
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

    private static bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
            return false;
        
        try
        {
            var mailAddress = new MailAddress(email);
            return mailAddress.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
}