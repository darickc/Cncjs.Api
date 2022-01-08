using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class Workflow
{
    private readonly CncJsSocketIo _client;

    private const    string        State = "workflow:state";

    public Action OnState { get; set; }
    internal Workflow(CncJsSocketIo client)
    {
        _client = client;
        _client.On(State, response => OnState?.Invoke());
    }

}