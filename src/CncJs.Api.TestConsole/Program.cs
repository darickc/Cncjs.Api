// See https://aka.ms/new-console-template for more information

using Cncjs.Api;
using CncJs.Api.TestConsole;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

CreateHostBuilder(args).Build().Run();


static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder =>
        {
            builder.AddUserSecrets<Program>()
                .AddJsonFile("secrets.json",true);
        })
        .ConfigureServices((hostContext, services) =>
    {
        CncJsOptions options = new CncJsOptions();
        hostContext.Configuration.Bind(options);
        services.AddSingleton(options);
        services.AddHostedService<App>();
    });
