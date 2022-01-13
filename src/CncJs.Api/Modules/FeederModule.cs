namespace CncJs.Api.Modules;

public class FeederModule
{
    private readonly CncJsClient _client;
    private const    string      Status = "feeder:status";
    public event EventHandler OnStatus;
    internal FeederModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(Status, _=> OnStatus?.Invoke(this, EventArgs.Empty));
    }

}