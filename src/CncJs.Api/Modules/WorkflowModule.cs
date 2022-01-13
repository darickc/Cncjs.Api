namespace CncJs.Api.Modules;

public class WorkflowModule
{
    private readonly CncJsClient _client;

    private const    string        State = "workflow:state";

    public event EventHandler OnState;
    internal WorkflowModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(State, response => OnState?.Invoke(this, EventArgs.Empty));
    }

}