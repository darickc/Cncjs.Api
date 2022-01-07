using Cncjs.Api.Models;
using Microsoft.Extensions.Logging;

namespace Cncjs.Api;

public class CncJsOptions
{
    public string Secret { get; set; }
    public string Name { get; set; } = "CncJs.Api";
    public int SocketPort { get; set; } = 8080;
    public string SocketAddress { get; set; } = "localhost";
    public int AccessTokenLifetime { get; set; } = 30;
    //public ControllerModel Controller { get; set; }
}