using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class Feeder
{
    private readonly CncJsSocketIo _client;
    private const    string        Status = "feeder:status";
    public Action OnStatus { get; set; }
    internal Feeder(CncJsSocketIo client)
    {
        _client = client;
        _client.On(Status, OnStatusEvent);
    }
    private void OnStatusEvent(SocketIOResponse obj)
    {
        OnStatus?.Invoke();
    }

}