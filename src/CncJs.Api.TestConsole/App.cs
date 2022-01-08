using Cncjs.Api;
using Cncjs.Api.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CncJs.Api.TestConsole;

public class App : BackgroundService, IDisposable
{
    private readonly Cncjs.Api.CncJs _cnc;

    public App(ILogger<App> logger, CncJsOptions options)
    {
        _cnc = new Cncjs.Api.CncJs(options, logger);
        _cnc.SerialPort.OnList = async list =>
        {
            if (list.Any())
            {
                var controller = new ControllerModel(list.First().Port, ControllerTypes.Grbl);
                await _cnc.SerialPort.OpenAsync(controller);
                var filesResult = await _cnc.Watch.GetFiles();
                // await Task.Delay(10000);
                // await _cnc.SerialPort.CloseAsync(controller);
            }
        };

        _cnc.OnConnected = async () =>
        {
            await _cnc.SerialPort.ListPortsAsync();
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