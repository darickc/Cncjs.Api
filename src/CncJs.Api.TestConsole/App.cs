using CncJs.Api.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CncJs.Api.TestConsole;

public class App : BackgroundService, IDisposable
{
    private readonly CncJsClient _cnc;

    public App(ILogger<App> logger, CncJsClient client)
    {
        _cnc = client;
        _cnc.OnConnected += (_, _) =>
        {

        };

        // _cnc.SerialPortModule.OnList = async list =>
        // {
        //     if (list.Any())
        //     {
        //         var controller = new Controller(list.First().Port, ControllerTypes.Grbl);
        //         await _cnc.SerialPortModule.OpenAsync(controller);
        //         var filesResult = await _cnc.WatchModule.GetFiles();
        //         // await Task.Delay(10000);
        //         // await _cnc.SerialPort.CloseAsync(controller);
        //     }
        // };
        //
        // _cnc.OnConnected = async () =>
        // {
        //     await _cnc.SerialPortModule.ListPortsAsync();
        // };
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _cnc.ConnectAsync();
    }
    
}