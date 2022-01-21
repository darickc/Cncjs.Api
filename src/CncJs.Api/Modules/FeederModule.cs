using CncJs.Api.Models;

namespace CncJs.Api.Modules;

public class FeederModule
{
    private readonly CncJsClient _client;
    private const    string      StatusEvent = "feeder:status";

    public FeederStatus Status { get; private set; }

    public event EventHandler<FeederStatus> OnStatus;

    internal FeederModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(StatusEvent, response=>
        {
            Status = response.GetValue<FeederStatus>();
            OnStatus?.Invoke(this, Status);
        });
    }

    internal void Clear()
    {
        Status = null;
    }

}