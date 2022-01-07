// using System.Threading.Tasks;
// using Cncjs.Api;
// using Cncjs.Api.Models;
// using Xunit;
//
// namespace CncJs.Api.Test;
//
// public class CncJsApiShould
// {
//     private readonly Cncjs.Api.CncJs _cnc;
//     public CncJsApiShould()
//     {
//         _cnc = new Cncjs.Api.CncJs(new CncJsOptions
//         {
//             Controller = new ControllerModel("/dev/ttyUSB0", ControllerTypes.Grbl),
//             SocketAddress = "192.168.0.227",
//             Secret = "$2a$10$8YQJh5K.WjZxlcL0/ff9C.",
//             SocketPort = 80
//         });
//     }
//
//     [Fact]
//     public async Task Connect()
//     {
//         _cnc.OnPortOpened = controller =>
//         {
//
//         };
//         _cnc.OnConnected = async () =>
//         {
//             await _cnc.Open();
//         };
//         await _cnc.ConnectAsync();
//         await Task.Delay(25000);
//     }
// }