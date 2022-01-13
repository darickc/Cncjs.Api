using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace CncJs.Api;

internal class CncJsSocketIo : SocketIO
{
    private readonly ILogger _logger;
    public CncJsSocketIo(string uri, ILogger logger) : base(uri)
    {
        _logger = logger;
        LogResponse();
    }

    public CncJsSocketIo(Uri uri, ILogger logger) : base(uri)
    {
        _logger = logger;
        LogResponse();
    }

    public CncJsSocketIo(string uri, SocketIOOptions options, ILogger logger) : base(uri, options)
    {
        _logger = logger;
        LogResponse();
    }

    public CncJsSocketIo(Uri uri, SocketIOOptions options, ILogger logger) : base(uri, options)
    {
        _logger = logger;
        LogResponse();
    }

    private void LogResponse()
    {
        OnAny((name, response) =>
        {
            _logger?.LogInformation($"Receiving: {name}: {response}");
        });
    }
     
    public new async Task EmitAsync(string eventName, params object[] data)
    {
        _logger?.LogInformation($"Sending: {eventName}: {JsonSerializer.Serialize(data).Json}");
        await base.EmitAsync(eventName, data);
    }
}