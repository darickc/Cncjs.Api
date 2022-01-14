using CncJs.Api.Models;

namespace CncJs.Api.Modules;

public class SerialPortModule
{
    private readonly CncJsClient _client;
    
    // responses
    private const     string   List   = "serialport:list";
    private const     string   Read   = "serialport:read";
    private const     string   Write   = "serialport:write";

    // commands
    private const string ListCommand = "list";
    private const string OpenCommand        = "open";
    private const string CloseCommand       = "close";
    private const string WriteCommand       = "write";

    public List<SerialPort> SerialPorts { get; set; } = new();

    public event EventHandler<List<SerialPort>> OnList;
    public event EventHandler<string> OnRead;
    public event EventHandler<string> OnWrite;

    internal SerialPortModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(List, response =>
        {
            SerialPorts = response.GetValue<List<SerialPort>>();
            OnList?.Invoke(this, SerialPorts);
            _client.OnPropertyChanged(nameof(SerialPorts));
        });
        _client.SocketIoClient.On(Read, response => OnRead?.Invoke(this, response.GetValue<string>()));
        _client.SocketIoClient.On(Write, response => OnWrite?.Invoke(this, response.GetValue<string>()));
    }
    
    public async Task ListPortsAsync()
    {
        await _client.SocketIoClient.EmitAsync(ListCommand);
    }

    public async Task OpenAsync(Controller controller)
    {
        await _client.SocketIoClient.EmitAsync(OpenCommand, controller.Port, new
        {
            baudrate = controller.Baudrate,
            controllerType = controller.ControllerType
        });
    }

    public async Task OpenAsync(string port)
    {
        await _client.SocketIoClient.EmitAsync(OpenCommand, port);
    }

    public async Task CloseAsync(Controller controller)
    {
        if (!_client.ControllerModule.ControllerConnected && !_client.Connected)
            return;
        await _client.SocketIoClient.EmitAsync(CloseCommand, _client.ControllerModule.Controller.Port);
    }

    public async Task SendRawAsync(params string[] cmd)
    {
        if (!_client.ControllerModule.ControllerConnected && !_client.Connected)
            return;
        await _client.SocketIoClient.EmitAsync(WriteCommand, _client.ControllerModule.Controller.Port, cmd.Length == 1 ? cmd[0] : cmd);
    }
}