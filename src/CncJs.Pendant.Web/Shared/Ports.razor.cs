using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Ports
    {
        [Inject] public CncJsClient Client { get; set; }
        public int[] Baudrates { get; set; } = { 250000,115200,57600,38400,19200,9600,4800,2400 };
        public string[] ControllerTypes { get; set; } = Api.Models.ControllerTypes.AsList;

        public int SelectedBaudrate { get; set; } = 115200;
        public string SelectedControllerType { get; set; }
        public SerialPort SelectedSerialPort { get; set; }


        protected override void OnInitialized()
        {
            SelectedControllerType = ControllerTypes.FirstOrDefault();
            SelectedSerialPort ??= 
                Client.SerialPortModule.SerialPorts.FirstOrDefault(p=>p.InUse) ?? 
                Client.SerialPortModule.SerialPorts.FirstOrDefault();
        }
        private async Task Connect()
        {
            var controller = new Controller(SelectedSerialPort.Port, SelectedControllerType, SelectedBaudrate);
            await Client.SerialPortModule.OpenAsync(controller);
        }
    }
}
