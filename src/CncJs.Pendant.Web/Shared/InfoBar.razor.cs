using Cncjs.Api;
using Cncjs.Api.Models;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class InfoBar
    {
        [Inject] public CncJsClient Client { get; set; }
        [Parameter]
        public ControllerModel Controller { get; set; }
        [Parameter]
        public ControllerState ControllerState { get; set; }

        public async Task Command(string cmd)
        {
            switch (cmd)
            {
                case "Unlock":
                    await Client.Controller.UnlockAsync(Controller.Port);
                    break;
                case "Reset":
                    await Client.Controller.ResetAsync(Controller.Port);
                    break;
                case "Home":
                    await Client.Controller.HomeAsync(Controller.Port);
                    break;
                case "Cyclestart":
                    await Client.Controller.CyclestartAsync(Controller.Port);
                    break;
                case "Feedhold":
                    await Client.Controller.FeedholdAsync(Controller.Port);
                    break;
            }
        }
    }
}
