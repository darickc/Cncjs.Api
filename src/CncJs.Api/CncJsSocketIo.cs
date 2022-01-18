using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SocketIOClient;
using SocketIOClient.JsonSerializer;

namespace CncJs.Api;

internal class CncJsSocketIo : SocketIO
{
    private readonly ILogger      _logger;
    private readonly CncJsOptions _options;
    private readonly string       _accessToken;

    public CncJsSocketIo(CncJsOptions options, ILogger logger) : base(options.WebSocketUrl)
    {
        _options = options;
        _logger = logger;
        _accessToken = string.IsNullOrEmpty(options.Token) ? GenerateAuthToken() : options.Token;
        Options.Query = new KeyValuePair<string, string>[] { new("token", _accessToken) };
        Options.EIO = 3;
        Options.Reconnection = true;

        if (JsonSerializer is SystemTextJsonSerializer jsonSerializer)
        {
            jsonSerializer.OptionsProvider = () => new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        LogResponse();
    }

    private string GenerateAuthToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Secret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new("id", ""),
                new("name", _options.Name),
            }),
            Expires = DateTime.UtcNow.AddDays(_options.AccessTokenLifetime),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    private void LogResponse()
    {
        OnAny((name, response) =>
        {
            _logger?.LogInformation($"Receiving: '{name}': {response}");
        });
    }
     
    public new async Task EmitAsync(string eventName, params object[] data)
    {
        _logger?.LogInformation($"Sending: {eventName}: {JsonSerializer.Serialize(data).Json}");
        await base.EmitAsync(eventName, data);
    }

    internal Task<Result<T>> Get<T>(string path, params KeyValuePair<string, string>[] query)
    {
        var temp = query.ToList();
        temp.Add(new KeyValuePair<string, string>("token", _accessToken));
        var url = $"{_options.ApiUrl}{path}?{string.Join("&", temp.Select(t => $"{t.Key}={t.Value}"))}";
        _logger.LogInformation($"Retrieving: {url}");
        return Result.Try(() => HttpClient.GetFromJsonAsync<T>(url));
    }
}