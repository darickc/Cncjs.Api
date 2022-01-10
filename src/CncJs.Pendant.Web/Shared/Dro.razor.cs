using Cncjs.Api;
using Cncjs.Api.Models;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Dro
    {
        [Parameter]
        public ControllerState ControllerState { get; set; }
        [Parameter]
        public Units MachineUnits { get; set; }

        public State State => ControllerState?.State;

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

    }
}
