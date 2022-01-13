using CncJs.Api.Models;

namespace CncJs.Api.Modules;

public class SerialPortModule
{
    private readonly CncJsClient _client;
    
    // responses
    private const     string   List   = "serialport:list";
    private const     string   Change = "serialport:change";
    private const     string   Open   = "serialport:open";
    private const     string   Close  = "serialport:close";
    private const     string   Error  = "serialport:error";
    private const     string   Read   = "serialport:read";
    private const     string   Write   = "serialport:write";

    // commands
    private const string ListCommand = "list";
    private const string OpenCommand        = "open";
    private const string CloseCommand       = "close";
    private const string WriteCommand       = "write";

    public event EventHandler<List<SerialPort>> OnList;
    public event EventHandler<Controller> OnChange;
    public event EventHandler<Controller> OnOpen;
    public event EventHandler<Controller> OnClose;
    public event EventHandler<Controller> OnError;
    public event EventHandler<string> OnRead;
    public event EventHandler<string> OnWrite;

    internal SerialPortModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(List, response => OnList?.Invoke(this, response.GetValue<List<SerialPort>>()));
        _client.SocketIoClient.On(Change, response => OnChange?.Invoke(this, response.GetValue<Controller>()));
        _client.SocketIoClient.On(Open, response => OnOpen?.Invoke(this, response.GetValue<Controller>()));
        _client.SocketIoClient.On(Close, response => OnClose?.Invoke(this, response.GetValue<Controller>()));
        _client.SocketIoClient.On(Error, response => OnError?.Invoke(this, response.GetValue<Controller>()));
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

    public async Task CloseAsync(Controller controller)
    {
        if (_client.ControllerConnected)
            return;
        await _client.SocketIoClient.EmitAsync(CloseCommand, _client.Controller.Port);
    }

    public async Task SendRawAsync(params string[] cmd)
    {
        if (_client.ControllerConnected)
            return;
        await _client.SocketIoClient.EmitAsync(WriteCommand, _client.Controller.Port, cmd.Length == 1 ? cmd[0] : cmd);
    }
}