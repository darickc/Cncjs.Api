namespace CncJs.Api.Modules;

public class SenderModule
{
    private readonly CncJsClient _client;
    private const    string      Status = "sender:status";
    public event EventHandler OnStatus;
    internal SenderModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(Status, _=> OnStatus?.Invoke(this, EventArgs.Empty));
    }

}