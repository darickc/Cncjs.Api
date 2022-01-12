using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Cncjs.Api.Models;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SocketIOClient;
using SocketIOClient.JsonSerializer;

namespace Cncjs.Api;
public class CncJsClient : IDisposable
{
    private readonly ILogger       _logger;
    private          CncJsSocketIo _client;
    private          HttpClient    _httpClient;
    private string _accessToken;

    // responses
    private const    string   Startup = "startup";
    private const    string   Message = "message";
    
    public CncJsOptions Options { get; set; }
    public bool Connected => _client?.Connected ?? false;

    public Func<Task> OnConnected { get; set; }
    public Func<Task> OnDisconnected { get; set; }
    public Action<string> OnError { get; set; }

    public Action<StartupModel> OnStartup { get; set; }
    public Action OnMessage { get; set; }

    public CncTask Task { get; private set; }
    public Config Config { get; private set; }
    public Controller Controller { get; private set; }
    public Feeder Feeder { get; private set; }
    public Gcode Gcode { get; private set; }
    public Sender Sender { get; private set; }
    public SerialPort SerialPort { get; private set; }
    public Workflow Workflow { get; private set; }
    public Watch Watch { get; private set; }
    public Macro Macro { get; private set; }

    public CncJsClient(CncJsOptions options, ILogger<CncJsClient> logger)
    {
        logger.LogInformation($"Creating CncJsClient with options: {JsonSerializer.Serialize(options)}");
        Options = options;
        _logger = logger;
        Initialize();
    }

    private string GenerateAuthToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Options.Secret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new("id", ""),
                new("name", Options.Name),
            }),
            Expires = DateTime.UtcNow.AddDays(Options.AccessTokenLifetime),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public void Initialize()
    {
        if (Options == null)
        {
            OnError?.Invoke("Options not set!");
            return;
        }
        _accessToken = GenerateAuthToken();
        
        _client = new CncJsSocketIo(Options.WebSocketUrl, new SocketIOOptions
        {
            Query = new KeyValuePair<string, string>[] { new("token", _accessToken) },
            EIO = 3,
            Reconnection = true
        }, _logger);
        if (_client.JsonSerializer is SystemTextJsonSerializer jsonSerializer)
        {
            jsonSerializer.OptionsProvider = () => new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Options.ApiUrl) 
        };

        Task = new CncTask(_client);
        Controller = new Controller(_client);
        Feeder = new Feeder(_client);
        Gcode = new Gcode(_client);
        Sender = new Sender(_client);
        SerialPort = new SerialPort(_client);
        Workflow = new Workflow(_client);
        Watch = new Watch(_client, this);
        Macro = new Macro(_client,this);
        HandleEvents();
    }

    private void HandleEvents()
    {
        _client.OnConnected += (_, _) =>
        {
            _logger?.LogInformation($"Connected to {Options.WebSocketUrl}");
            OnConnected?.Invoke();
        };
        _client.OnError += (_, e) =>
        {
            _logger?.LogInformation($"Error from {Options.WebSocketUrl}: {e}");
            OnError?.Invoke(e);
        };
        _client.OnDisconnected += (_, _) =>
        {
            _logger?.LogInformation($"Disconnected from {Options.WebSocketUrl}");
            OnDisconnected?.Invoke();
        };

        _client.On(Startup, OnStartupEvent);
        _client.On(Message, OnMessageEvent);
    }

    private void OnStartupEvent(SocketIOResponse obj)
    {
        var model = obj.GetValue<StartupModel>();
        OnStartup?.Invoke(model);
    }

    private void OnMessageEvent(SocketIOResponse obj)
    {
        OnMessage?.Invoke();
    }

    public async Task ConnectAsync()
    {
        if (_client == null)
        {
            OnError?.Invoke("Client is not created, call CreateClient or pass options into the constructor.");
            return;
        }
        await _client.ConnectAsync();
    }

    public async Task DisconnectAsync()
    {
        if (Connected)
        {
            _logger?.LogInformation("Disconnecting");
            await _client.DisconnectAsync();
        }
    }

    internal Task<Result<T>> Get<T>(string path, params KeyValuePair<string,string>[] query)
    {
        var temp = query.ToList();
        temp.Add(new KeyValuePair<string, string>("token", _accessToken));
        var url = $"{Options.ApiUrl}{path}?{string.Join("&", temp.Select(t => $"{t.Key}={t.Value}"))}";
        return Result.Try(() => _httpClient.GetFromJsonAsync<T>(url));
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

