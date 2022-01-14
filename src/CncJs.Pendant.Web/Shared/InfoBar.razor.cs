using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class InfoBar : IDisposable
    {
        [Inject] public CncJsClient Client { get; set; }

        protected override void OnInitialized()
        {
            Client.ControllerModule.OnState += ControllerModuleOnOnState;
        }

        private void ControllerModuleOnOnState(object sender, ControllerState e)
        {
            InvokeAsync(StateHasChanged);
        }

        public async Task Command(string cmd)
        {
            switch (cmd)
            {
                case "Unlock":
                    await Client.ControllerModule.UnlockAsync();
                    break;
                case "Reset":
                    await Client.ControllerModule.ResetAsync();
                    break;
                case "Home":
                    await Client.ControllerModule.HomeAsync();
                    break;
                case "Cyclestart":
                    await Client.ControllerModule.CyclestartAsync();
                    break;
                case "Feedhold":
                    await Client.ControllerModule.FeedholdAsync();
                    break;
            }
        }

        public void Dispose()
        {
            Client.ControllerModule.OnState -= ControllerModuleOnOnState;
        }
    }
}
