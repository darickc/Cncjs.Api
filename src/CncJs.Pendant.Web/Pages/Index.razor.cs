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
            // Client.PropertyChanged += Client_PropertyChanged;
            Client.OnError += Client_OnError;
            Client.ControllerModule.OnState += ControllerModule_OnState;
            Client.ControllerModule.OnSettings += ControllerModule_OnSettings;
            Client.OnConnected += Client_OnConnected;
            Client.OnDisconnected += ClientOnOnDisconnected;
            Client.ControllerModule.OnOpen += ControllerModule_OnOpen;
            Client.ControllerModule.OnClose += ControllerModule_OnClose;
            Client.ControllerModule.OnChange += ControllerModule_OnChange;
            Client.SerialPortModule.OnList += SerialPortModule_OnList;
            Load();
        }

        private void SerialPortModule_OnList(object sender, List<Api.Models.SerialPort> e)
        {
            Load();
        }

        private async void ControllerModule_OnChange(object sender, Api.Models.Controller e)
        {
            if (e.InUse)
            {
                await Client.SerialPortModule.OpenAsync(e.Port);
            }
            await InvokeAsync(StateHasChanged);
        }

        private void ControllerModule_OnClose(object sender, Api.Models.Controller e)
        {
            InvokeAsync(StateHasChanged);
        }

        private void ControllerModule_OnOpen(object sender, Api.Models.Controller e)
        {
            InvokeAsync(StateHasChanged);
        }

        private void ClientOnOnDisconnected(object sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        private void Client_OnConnected(object sender, EventArgs e)
        {
            Load();
        }

        private void ControllerModule_OnSettings(object sender, Api.Models.ControllerSettings e)
        {
            SetFeedrate();
            InvokeAsync(StateHasChanged);
        }

        private void ControllerModule_OnState(object sender, Api.Models.ControllerState e)
        {
            if (Feedrate != null)
            {
                Feedrate.Units = e.State.Units;
            }
            else
            {
                SetFeedrate();
            }
            Jogging.Units = e.State.Units;
            InvokeAsync(StateHasChanged);
        }

        private void SetFeedrate()
        {
            if (Feedrate == null && (Client.ControllerModule.ControllerSettings?.MaxFeedrate ?? 0) > 0 && Client.ControllerModule.ControllerState != null)
            {
                Feedrate = new FeedrateModel(Client.ControllerModule.ControllerSettings.MaxFeedrate, 
                    Client.ControllerModule.ControllerSettings.MachineUnits, 
                    Client.ControllerModule.ControllerState.State.Units);
            }
        }

        private void Client_OnError(object sender, string e)
        {
            Loading = false;
            Snackbar.Add(e, Severity.Error);
            InvokeAsync(StateHasChanged);
        }

        private async void Load()
        {
            
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
                SetFeedrate();
            }
            Loading = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task Connect()
        {
            Loading = true;
           await Client.ConnectAsync();
           try
           {

           }
           catch
           {
               Loading = false;
               await InvokeAsync(StateHasChanged);
            }
        }

        public void Dispose()
        {
            Client.OnConnected -= Client_OnConnected;
            Client.OnDisconnected -= ClientOnOnDisconnected;
            Client.OnError -= Client_OnError;
            Client.ControllerModule.OnState -= ControllerModule_OnState;
            Client.ControllerModule.OnSettings -= ControllerModule_OnSettings;
            Client.ControllerModule.OnOpen -= ControllerModule_OnOpen;
            Client.ControllerModule.OnClose -= ControllerModule_OnClose;
            Client.ControllerModule.OnChange -= ControllerModule_OnChange;
            Client.SerialPortModule.OnList -= SerialPortModule_OnList;
        }
    }
}
