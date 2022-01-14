using CncJs.Api.Models;

namespace CncJs.Api.Modules;

public class SenderModule
{
    private readonly CncJsClient _client;
    private const    string      StatusEvent = "sender:status";

    public SenderStatus Status { get; set; }

    public event EventHandler<SenderStatus> OnStatus;
    internal SenderModule(CncJsClient client)
    {
        _client = client;
        _client.SocketIoClient.On(StatusEvent, response=>
        {
            Status = response.GetValue<SenderStatus>();
            OnStatus?.Invoke(this, Status);
            _client.OnPropertyChanged("SenderStatus");
        });
    }

}