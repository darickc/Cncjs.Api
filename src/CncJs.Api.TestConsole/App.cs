using Cncjs.Api;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CncJs.Api.TestConsole;

public class App : BackgroundService, IDisposable
{
    private readonly Cncjs.Api.CncJs _cnc;

    public App(ILogger<App> logger, CncJsOptions options)
    {
        _cnc = new Cncjs.Api.CncJs(options, logger);
        
        _cnc.OnConnected = async () =>
        {
            await _cnc.SerialPort.ListPortsAsync();
            // await _cnc.OpenAsync(new ControllerModel("/dev/ttyUSB0", ControllerTypes.Grbl));
            // _cnc.Controller
        };
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _cnc.ConnectAsync();
    }

    public void Dispose()
    {
        _cnc.Dispose();
    }
}