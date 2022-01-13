using SocketIOClient;

namespace CncJs.Api.Modules;

public class TaskModule
{
    private readonly CncJsClient _client;
    private const    string      Start  = "task:start";
    private const    string      Finish = "task:finish";
    private const    string      Error  = "task:error";

    public event EventHandler OnStart;
    public event EventHandler OnFinish;
    public event EventHandler OnError;

    internal TaskModule(CncJsClient client)
    {
        _client = client;

        _client.SocketIoClient.On(Start, OnStartEvent);
        _client.SocketIoClient.On(Finish, OnFinishEvent);
        _client.SocketIoClient.On(Error, OnErrorEvent);
    }

    private void OnStartEvent(SocketIOResponse obj)
    {
        OnStart?.Invoke(this, EventArgs.Empty);
    }

    private void OnFinishEvent(SocketIOResponse obj)
    {
        OnFinish?.Invoke(this, EventArgs.Empty);
    }

    private void OnErrorEvent(SocketIOResponse obj)
    {
        OnError?.Invoke(this, EventArgs.Empty);
    }
}