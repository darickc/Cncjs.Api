using Cncjs.Api;
using Cncjs.Api.Models;
using Microsoft.Extensions.Logging;

namespace CncJs.Api.TestConsole;

public class App : IDisposable
{
    private readonly Cncjs.Api.CncJs _cnc;

    public App(ILogger<App> logger)
    {
        _cnc = new Cncjs.Api.CncJs(new CncJsOptions
        {
            //Controller = new ControllerModel("/dev/ttyUSB0", ControllerTypes.Grbl),
            SocketAddress = "192.168.0.227",
            Secret = "$2a$10$8YQJh5K.WjZxlcL0/ff9C.",
            SocketPort = 80,
        }, logger);
        
        _cnc.OnConnected = async () =>
        {
            //await _cnc.SerialPort.ListPortsAsync();
             await _cnc.OpenAsync(new ControllerModel("/dev/ttyUSB0", ControllerTypes.Grbl));
            // _cnc.Controller
        };
    }

    public async Task Start()
    {
        await _cnc.ConnectAsync();
    }

    public void Dispose()
    {
        _cnc.Dispose();
    }
}