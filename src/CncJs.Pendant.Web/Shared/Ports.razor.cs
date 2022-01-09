using Cncjs.Api;
using Cncjs.Api.Models;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Ports
    {
        [Inject] public CncJsClient Client { get; set; }
        public List<SerialPortModel> AvailablePorts { get; set; } = new();
        public int[] Baudrates { get; set; } = { 250000,115200,57600,38400,19200,9600,4800,2400 };
        public string[] ControllerTypes { get; set; } = Cncjs.Api.ControllerTypes.AsList;

        public int SelectedBaudrate { get; set; } = 115200;
        public string SelectedControllerType { get; set; } = Cncjs.Api.ControllerTypes.AsList.First();
        public SerialPortModel SelectedSerialPort { get; set; }
        public bool Loading { get; set; }


        protected override async Task OnInitializedAsync()
        {
            Client.SerialPort.OnList = OnList;
            Client.OnConnected = OnConnected;
            if (!Client.Connected)
            {
                await Client.ConnectAsync();
            }
            else
            {
                Loading = true;
                await Client.SerialPort.ListPortsAsync();
            }
        }

        private async Task OnList(List<SerialPortModel> arg)
        {
            AvailablePorts = arg;
            SelectedSerialPort = AvailablePorts.FirstOrDefault(p=>p.InUse) ?? AvailablePorts.FirstOrDefault();
            Loading = false;
            if (SelectedSerialPort?.InUse == true)
            {
                await Connect();
            }
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnConnected()
        {
            Loading = true;
            await Client.SerialPort.ListPortsAsync();
        }

        private async Task Connect()
        {
            var controller = new ControllerModel(SelectedSerialPort.Port, SelectedControllerType, SelectedBaudrate);
            await Client.SerialPort.OpenAsync(controller);
        }
    }
}
