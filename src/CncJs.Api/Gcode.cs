using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class Gcode
{
    private readonly CncJsSocketIo _client;
    private const    string        Load   = "gcode:load";
    private const    string        UnLoad = "gcode:unload";

    public Action OnLoad { get; set; }
    public Action OnUnLoad { get; set; }

    internal Gcode(CncJsSocketIo client)
    {
        _client = client;

        _client.On(Load, OnLoadEvent);
        _client.On(UnLoad, OnUnLoadEvent);
    }
    private void OnLoadEvent(SocketIOResponse obj)
    {
        OnLoad?.Invoke();
    }

    private void OnUnLoadEvent(SocketIOResponse obj)
    {
        OnUnLoad?.Invoke();
    }

}