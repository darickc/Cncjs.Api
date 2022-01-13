using System.Net.Http.Json;
using CSharpFunctionalExtensions;

namespace CncJs.Api;

internal class CncJsHttpClient : HttpClient
{
    private readonly string _accessToken;
    public CncJsHttpClient(string apiUrl, string accessToken)
    {
        _accessToken = accessToken;
        BaseAddress = new Uri(apiUrl);
    }
    internal Task<Result<T>> Get<T>(string path, params KeyValuePair<string, string>[] query)
    {
        var temp = query.ToList();
        temp.Add(new KeyValuePair<string, string>("token", _accessToken));
        var url = $"{path}?{string.Join("&", temp.Select(t => $"{t.Key}={t.Value}"))}";
        return Result.Try(() => this.GetFromJsonAsync<T>(url));
    }
}