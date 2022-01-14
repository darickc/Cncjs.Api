namespace CncJs.Api.Modules;

public class WorkflowModule
{
    private readonly CncJsClient _client;

    private const    string        WorflowState = "workflow:state";

    public string State { get; set; }

    public event EventHandler<string> OnState;
    internal WorkflowModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(WorflowState, response =>
        {
            State = response.GetValue<string>();
            OnState?.Invoke(this, State);
            _client.OnPropertyChanged("WorkflowState");
        });
    }

}