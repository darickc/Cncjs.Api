using Cncjs.Api.Models;
using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class SerialPort
{
    private readonly CncJsSocketIo _client;
    
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

    public Func<List<SerialPortModel>, Task> OnList { get; set; }
    public Func<ControllerModel, Task> OnChange { get; set; }
    public Func<ControllerModel, Task> OnOpen { get; set; }
    public Func<ControllerModel, Task> OnClose { get; set; }
    public Func<ControllerModel, Task> OnError { get; set; }
    public Func<string, Task> OnRead { get; set; }
    public Func<string, Task> OnWrite { get; set; }

    internal SerialPort(CncJsSocketIo client)
    {
        _client = client;
        _client.On(List, response => OnList?.Invoke(response.GetValue<List<SerialPortModel>>()));
        _client.On(Change, response => OnChange?.Invoke(response.GetValue<ControllerModel>()));
        _client.On(Open, response => OnOpen?.Invoke(response.GetValue<ControllerModel>()));
        _client.On(Close, response => OnClose?.Invoke(response.GetValue<ControllerModel>()));
        _client.On(Error, response => OnError?.Invoke(response.GetValue<ControllerModel>()));
        _client.On(Read, response => OnRead?.Invoke(response.GetValue<string>()));
        _client.On(Write, response => OnWrite?.Invoke(response.GetValue<string>()));
    }
    
    public async Task ListPortsAsync()
    {
        await _client.EmitAsync(ListCommand);
    }

    public async Task OpenAsync(ControllerModel controller)
    {
        await _client.EmitAsync(OpenCommand, controller.Port, new
        {
            baudrate = controller.Baudrate,
            controllerType = controller.ControllerType
        });
    }

    public async Task CloseAsync(ControllerModel controller)
    {
        await _client.EmitAsync(CloseCommand, controller.Port);
    }

    public async Task SendRawAsync(string port, params char[] cmd)
    {
        await _client.EmitAsync(WriteCommand, port, cmd);
    }
}