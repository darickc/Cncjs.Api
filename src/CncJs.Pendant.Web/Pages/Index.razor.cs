using System.ComponentModel;
using CncJs.Api;
using CncJs.Pendant.Web.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Pages
{
    public partial class Index : IDisposable
    {
        [Inject] public CncJsClient Client { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        public bool Loading { get; set; }
        public JoggingModel Jogging { get; set; } = new();
        public FeedrateModel Feedrate { get; set; }

        protected override void OnInitialized()
        {
            Client.PropertyChanged += Client_PropertyChanged;
            Client.OnError += Client_OnError;
            Client.ControllerModule.OnState += ControllerModule_OnState;
            Client_PropertyChanged(null, null);
        }

        private void ControllerModule_OnState(object sender, Api.Models.ControllerState e)
        {
            if (Feedrate != null)
            {
                Feedrate.Units = e.State.Units;
            }
            Jogging.Units = e.State.Units;
            InvokeAsync(StateHasChanged);
        }

        private void Client_OnError(object sender, string e)
        {
            Loading = false;
            Snackbar.Add(e, Severity.Error);
        }

        private async void Client_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Loading = false;
            if (!Client.Connected)
            {
                await Connect();
            }
            else if (!Client.ControllerModule.ControllerConnected && !Client.SerialPortModule.SerialPorts.Any())
            {
                Loading = true;
                await Client.SerialPortModule.ListPortsAsync();
            }
            else if (!Client.ControllerModule.ControllerConnected && Client.SerialPortModule.SerialPorts.Any(p=>p.InUse))
            {
                var port = Client.SerialPortModule.SerialPorts.First(p => p.InUse);
                await Client.SerialPortModule.OpenAsync(port.Port);
            }
            else if (Feedrate == null && Client.ControllerModule.ControllerSettings != null)
            {
                Feedrate = new FeedrateModel(Client.ControllerModule.ControllerSettings.MaxFeedrate,
                    Client.ControllerModule.ControllerSettings.MachineUnits);
            }

            await InvokeAsync(StateHasChanged);
        }

        private async Task Connect()
        {
            Loading = true;
           await Client.ConnectAsync();
        }

        public void Dispose()
        {
            Client.PropertyChanged -= Client_PropertyChanged;
        }
    }
}
