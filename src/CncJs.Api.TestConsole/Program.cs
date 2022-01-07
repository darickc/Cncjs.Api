// See https://aka.ms/new-console-template for more information

using CncJs.Api.TestConsole;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
ConfigureServices(services);
ServiceProvider serviceProvider = services.BuildServiceProvider();
var app = serviceProvider.GetService<App>();

await app.Start();

Console.ReadLine();


void ConfigureServices(ServiceCollection services)
{
    services.AddLogging(configure => configure.AddConsole())
        .AddTransient<App>();
}