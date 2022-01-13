namespace CncJs.Api.Modules;

public class ConfigModule
{
    private readonly CncJsClient _client;
    private const    string      Change = "config:change";
    public event EventHandler OnChange;
    internal ConfigModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(Change, _=> OnChange?.Invoke(this, EventArgs.Empty));
    }

}