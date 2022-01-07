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

    // commands
    private const string ListCommand = "list";

    public Action<List<SerialPortModel>> OnList { get; set; }
    public Action OnChange { get; set; }
    public Action<ControllerModel> OnOpen { get; set; }
    public Action OnClose { get; set; }
    public Action<ControllerModel> OnError { get; set; }
    public Action OnRead { get; set; }

    internal SerialPort(CncJsSocketIo client)
    {
        _client = client;
        _client.On(List, OnListEvent);
        _client.On(Change, OnChangeEvent);
        _client.On(Open, OnOpenEvent);
        _client.On(Close, OnCloseEvent);
        _client.On(Error, OnErrorEvent);
        _client.On(Read, OnReadEvent);
    }

    private void OnListEvent(SocketIOResponse obj)
    {
        var portsList = obj.GetValue<List<SerialPortModel>>();
        OnList?.Invoke(portsList);
    }

    private void OnChangeEvent(SocketIOResponse obj)
    {
        OnChange?.Invoke();
    }

    private void OnOpenEvent(SocketIOResponse obj)
    {
        var controller = obj.GetValue<ControllerModel>();
        OnOpen?.Invoke(controller);
    }

    private void OnCloseEvent(SocketIOResponse obj)
    {
        OnClose?.Invoke();
    }

    private void OnErrorEvent(SocketIOResponse obj)
    {
        var controller = obj.GetValue<ControllerModel>();
        OnError?.Invoke(controller);
    }

    private void OnReadEvent(SocketIOResponse obj)
    {
        OnRead?.Invoke();
    }

    public async Task ListPortsAsync()
    {
        await _client.EmitAsync(ListCommand);
    }
}