using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Ports : IDisposable
    {
        [Inject] public CncJsClient Client { get; set; }
        public int[] Baudrates { get; set; } = { 250000,115200,57600,38400,19200,9600,4800,2400 };
        public string[] ControllerTypes { get; set; } = Api.Models.ControllerTypes.AsList;

        public int SelectedBaudrate { get; set; } = 115200;
        public string SelectedControllerType { get; set; }
        public SerialPort SelectedSerialPort { get; set; }


        protected override void OnInitialized()
        {
            Client.SerialPortModule.OnList += SerialPortModuleOnOnList;

            SelectedControllerType = ControllerTypes.FirstOrDefault();
            SerialPortModuleOnOnList(null, null);
        }

        private async void SerialPortModuleOnOnList(object sender, List<SerialPort> e)
        {
            SelectedSerialPort ??=
                Client.SerialPortModule.SerialPorts.FirstOrDefault(p => p.InUse) ??
                Client.SerialPortModule.SerialPorts.FirstOrDefault();
            await InvokeAsync(StateHasChanged);
        }

        private async Task Connect()
        {
            var controller = new Controller(SelectedSerialPort.Port, SelectedControllerType, SelectedBaudrate);
            await Client.SerialPortModule.OpenAsync(controller);
        }

        public void Dispose()
        {
            Client.SerialPortModule.OnList -= SerialPortModuleOnOnList;
        }
    }
}
