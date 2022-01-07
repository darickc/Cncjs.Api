using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class CncTask
{
    private readonly SocketIO _client;
    private const string Start = "task:start";
    private const string Finish = "task:finish";
    private const string Error = "task:error";

    public Action OnStart { get; set; }
    public Action OnFinish { get; set; }
    public Action OnError { get; set; }

    internal CncTask(SocketIO client)
    {
        _client = client;

        _client.On(Start, OnStartEvent);
        _client.On(Finish, OnFinishEvent);
        _client.On(Error, OnErrorEvent);
    }

    private void OnStartEvent(SocketIOResponse obj)
    {
        OnStart?.Invoke();
    }

    private void OnFinishEvent(SocketIOResponse obj)
    {
        OnFinish?.Invoke();
    }

    private void OnErrorEvent(SocketIOResponse obj)
    {
        OnError?.Invoke();
    }
}