using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CncJs.Api.Annotations;
using CncJs.Api.Models;
using CncJs.Api.Modules;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SocketIOClient;
using SocketIOClient.JsonSerializer;

namespace CncJs.Api;
public class CncJsClient : IDisposable, INotifyPropertyChanged
{
    private readonly ILogger            _logger;
    private          string             _accessToken;
    private          Controller         _controller;
    private          ControllerSettings _controllerSettings;
    private          ControllerState    _controllerState;

    // responses
    private const    string   Startup = "startup";
    private const    string   Message = "message";
    
    // properties
    public CncJsOptions Options { get; set; }
    public bool Connected => SocketIoClient?.Connected ?? false;
    internal CncJsSocketIo SocketIoClient { get; private set; }
    internal CncJsHttpClient HttpClient { get; private set; }

    public Controller Controller
    {
        get => _controller;
        set
        {
            if (Equals(value, _controller)) return;
            _controller = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ControllerConnected));
        }
    }

    public ControllerSettings ControllerSettings
    {
        get => _controllerSettings;
        set
        {
            if (Equals(value, _controllerSettings)) return;
            _controllerSettings = value;
            OnPropertyChanged();
        }
    }

    public ControllerState ControllerState
    {
        get => _controllerState;
        set
        {
            if (Equals(value, _controllerState)) return;
            _controllerState = value;
            OnPropertyChanged();
        }
    }

    public bool ControllerConnected => Connected && Controller != null;

    // events
    public event EventHandler OnConnected;
    public event EventHandler OnDisconnected;
    public event EventHandler<string> OnError;
    public event EventHandler<Startup> OnStartup;
    public event EventHandler OnMessage;
    public event PropertyChangedEventHandler PropertyChanged;

    // Modules
    public TaskModule TaskModule { get; private set; }
    public ConfigModule ConfigModule { get; private set; }
    public ControllerModule ControllerModule { get; private set; }
    public FeederModule FeederModule { get; private set; }
    public GcodeModule GcodeModule { get; private set; }
    public SenderModule SenderModule { get; private set; }
    public SerialPortModule SerialPortModule { get; private set; }
    public WorkflowModule WorkflowModule { get; private set; }
    public WatchModule WatchModule { get; private set; }
    public MacroModule MacroModule { get; private set; }

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
            OnError?.Invoke(this,"Options not set!");
            return;
        }
        _accessToken = GenerateAuthToken();

        SocketIoClient = new CncJsSocketIo(Options.WebSocketUrl, new SocketIOOptions
        {
            Query = new KeyValuePair<string, string>[] { new("token", _accessToken) },
            EIO = 3,
            Reconnection = true
        }, _logger);
        if (SocketIoClient.JsonSerializer is SystemTextJsonSerializer jsonSerializer)
        {
            jsonSerializer.OptionsProvider = () => new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        HttpClient = new CncJsHttpClient(Options.ApiUrl, _accessToken);

        TaskModule = new TaskModule(this);
        ControllerModule = new ControllerModule(this);
        FeederModule = new FeederModule(this);
        GcodeModule = new GcodeModule(this);
        SenderModule = new SenderModule(this);
        SerialPortModule = new SerialPortModule(this);
        WorkflowModule = new WorkflowModule(this);
        WatchModule = new WatchModule(this);
        MacroModule = new MacroModule(this);
        HandleEvents();
    }

    private void HandleEvents()
    {
        SocketIoClient.OnConnected += (_, _) =>
        {
            _logger?.LogInformation($"Connected to {Options.WebSocketUrl}");
            OnConnected?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(ControllerConnected));
            OnPropertyChanged(nameof(Connected));
        };
        SocketIoClient.OnError += (_, e) =>
        {
            _logger?.LogInformation($"Error from {Options.WebSocketUrl}: {e}");
            OnError?.Invoke(this, e);
        };
        SocketIoClient.OnDisconnected += (_, _) =>
        {
            _logger?.LogInformation($"Disconnected from {Options.WebSocketUrl}");
            OnDisconnected?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(ControllerConnected));
            OnPropertyChanged(nameof(Connected));
        };

        SocketIoClient.On(Startup, OnStartupEvent);
        SocketIoClient.On(Message, OnMessageEvent);

        SerialPortModule.OnOpen += (_, controller) => Controller = controller;
        SerialPortModule.OnClose += (_, _) => Controller = null;
        SerialPortModule.OnChange += (_, controller) => Controller = controller;
        ControllerModule.OnSettings += (_, settings) => ControllerSettings = settings;
        ControllerModule.OnState += (_, state) => ControllerState = state;
    }

    private void OnStartupEvent(SocketIOResponse obj)
    {
        var model = obj.GetValue<Startup>();
        OnStartup?.Invoke(this, model);
    }

    private void OnMessageEvent(SocketIOResponse obj)
    {
        OnMessage?.Invoke(this, EventArgs.Empty);
    }

    public async Task ConnectAsync()
    {
        if (SocketIoClient == null)
        {
            OnError?.Invoke(this,"Client is not created, call CreateClient or pass options into the constructor.");
            return;
        }
        await SocketIoClient.ConnectAsync();
    }

    public async Task DisconnectAsync()
    {
        if (Connected)
        {
            _logger?.LogInformation("Disconnecting");
            await SocketIoClient.DisconnectAsync();
        }
    }
    
    public void Dispose()
    {
        SocketIoClient?.Dispose();
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

