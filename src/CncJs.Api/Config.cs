using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class Config
{
    private readonly CncJsSocketIo _client;
    private const    string        Change = "config:change";
    public Action OnChange { get; set; }
    internal Config(CncJsSocketIo client)
    {
        _client = client;
        _client.On(Change, OnChangeEvent);
    }
    private void OnChangeEvent(SocketIOResponse obj)
    {
        OnChange?.Invoke();
    }

}