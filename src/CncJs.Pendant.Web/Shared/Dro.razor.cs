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
            if (State?.Units == "MM" && MachineUnits == Units.Millimeters || State?.Units == "IN" && MachineUnits == Units.Inches)
            {
                return value;
            }

            if (MachineUnits == Units.Millimeters)
            {
                // convert to inches
                if (double.TryParse(value, out var mm))
                {
                    return (mm / 25.4).ToString("0.000");
                }
            }
            // convert to mm
            if (double.TryParse(value, out var inches))
            {
                return (inches * 25.4).ToString("0.000");
            }

            return value;
        }

    }
}
