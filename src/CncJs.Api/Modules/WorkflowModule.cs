namespace CncJs.Api.Modules;

public class WorkflowModule
{
    private readonly CncJsClient _client;

    private const string WorflowState = "workflow:state";
    private const string Start        = "start";
    private const string Stop         = "stop";
    private const string Pause        = "pause";
    private const string Resume       = "resume";
    private const string Command      = "command";

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

    public async Task SendCommandAsync(string cmd)
    {
        if (!_client.ControllerModule.ControllerConnected && !_client.Connected)
            return;
        await _client.SocketIoClient.EmitAsync(Command, _client.ControllerModule.Controller.Port, cmd);
    }

    public async Task StartAsync()
    {
        await SendCommandAsync(Start);
    }

    public async Task StopAsync()
    {
        await SendCommandAsync(Stop);
    }

    public async Task PauseAsync()
    {
        await SendCommandAsync(Pause);
    }

    public async Task ResumeAsync()
    {
        await SendCommandAsync(Resume);
    }
    
}