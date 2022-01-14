using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Dro : IDisposable
    {
        [Inject] public CncJsClient Client { get; set; }
        public Units MachineUnits => Client.ControllerModule.ControllerSettings?.MachineUnits ?? Units.Metric;

        public State State => Client.ControllerModule.ControllerState?.State;

        protected override void OnInitialized()
        {
            Client.ControllerModule.OnState += ControllerModuleOnOnState;
        }

        private void ControllerModuleOnOnState(object sender, ControllerState e)
        {
            InvokeAsync(StateHasChanged);
        }

        public string GetPosition(string value)
        {
            if (State?.Units == MachineUnits || State?.Units == MachineUnits)
            {
                return value;
            }

            if (MachineUnits == Units.Metric)
            {
                // convert to imperial
                if (double.TryParse(value, out var mm))
                {
                    return mm.ToImperial().ToString("0.000");
                }
            }
            // convert to metric
            if (double.TryParse(value, out var inches))
            {
                return inches.ToMetric().ToString("0.000");
            }

            return value;
        }

        public void Dispose()
        {
            Client.ControllerModule.OnState -= ControllerModuleOnOnState;
        }
    }
}
