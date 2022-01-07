using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Cncjs.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SocketIOClient;
using SocketIOClient.JsonSerializer;

namespace Cncjs.Api;
public class CncJs : IDisposable
{
    private readonly ILogger       _logger;
    private          CncJsSocketIo _client;

    // responses
    private const    string   Startup = "startup";
    private const    string   Message = "message";

    // commands
    private const string Open = "open";

    public CncJsOptions Options { get; set; }
    public bool Connected => _client?.Connected ?? false;


    public Func<Task> OnConnected { get; set; }
    public Action OnDisconnected { get; set; }
    public Action<string> OnError { get; set; }

    public Action<StartupModel> OnStartup { get; set; }
    public Action OnMessage { get; set; }
    public Action OnList { get; set; }

    public CncTask Task { get; set; }
    public Config Config { get; set; }
    public Controller Controller { get; set; }
    public Feeder Feeder { get; set; }
    public Gcode Gcode { get; set; }
    public Sender Sender { get; set; }
    public SerialPort SerialPort { get; set; }
    public Workflow Workflow { get; set; }

    public CncJs(CncJsOptions options, ILogger logger)
    {
        Options = options;
        _logger = logger;
        CreateClient();
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

    private void CreateClient()
    {
        if (Options == null)
        {
            OnError?.Invoke("Options not set!");
            return;
        }
        var token = GenerateAuthToken();

        var port = Options.SocketPort == 80 ? $":{Options.SocketPort}" : "";
        var url = $"ws://{Options.SocketAddress}{port}";

        _client = new CncJsSocketIo(url, new SocketIOOptions
        {
            Query = new KeyValuePair<string, string>[] { new("token", token) },
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

        Task = new CncTask(_client);
        Controller = new Controller(_client);
        Feeder = new Feeder(_client);
        Gcode = new Gcode(_client);
        Sender = new Sender(_client);
        SerialPort = new SerialPort(_client);
        Workflow = new Workflow(_client);

        HandleEvents(url);
    }

    private void HandleEvents(string url)
    {
        _client.OnConnected += (_, _) =>
        {
            _logger?.LogInformation($"Connected to {url}");
            OnConnected?.Invoke();
        };
        _client.OnError += (_, e) =>
        {
            _logger?.LogInformation($"Error from {url}: {e}");
            OnError?.Invoke(e);
        };
        _client.OnDisconnected += (_, _) =>
        {
            _logger?.LogInformation($"Disconnected from {url}");
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

            await _client.DisconnectAsync();
        }
    }

    public async Task OpenAsync(ControllerModel controller)
    {
        _logger?.LogInformation($"Sending Open: Port={controller.Port}, Baudrate={controller.Baudrate}, controllerType={controller.ControllerType}");
        await _client.EmitAsync(Open, controller.Port, new
        {
            baudrate = controller.Baudrate,
            controllerType = controller.ControllerType
        });
    }

    

    public void Dispose()
    {
        _client?.Dispose();
    }
}

