using Cncjs.Api;
using Cncjs.Api.Models;
using CncJs.Pendant.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Jogger
    {
        [Inject] public CncJsClient Client { get; set; }
        [Inject] public ILogger<Jogger> Logger { get; set; }
        [Parameter]
        public ControllerState ControllerState { get; set; }
        [Parameter]
        public ControllerModel Controller { get; set; }

        [Parameter]
        public JoggingModel Jogging { get; set; }

        private async Task Jog(string value)
        {
            if (ControllerState == null)
            {
                return;
            }

            var movementType = "G91";
            var letters = new[] { "X", "Y", "Z" };
            var temp = new List<string>();
            if (value.Contains("0"))
            {
                movementType = "G90";
                // temp.Add(value);
            }
            foreach (var letter in letters)
            {
                if (value.Contains($"{letter}+"))
                {
                    temp.Add($"{letter}{Jogging.Distance}");
                }
                if (value.Contains($"{letter}-"))
                {
                    temp.Add($"{letter}{-Jogging.Distance}");
                }
                if (value.Contains($"{letter}0"))
                {
                    temp.Add($"{letter}0");
                }
            }

            if (ControllerState.State.Parserstate.Modal.Distance != movementType)
            {
                await Client.Gcode.SendCommandAsync(Controller.Port, movementType);
                await Client.Gcode.SendCommandAsync(Controller.Port, $"G0 {string.Join(" ", temp)}");
                await Client.Gcode.SendCommandAsync(Controller.Port, ControllerState.State.Parserstate.Modal.Distance);
            }
            else
            {
                await Client.Gcode.SendCommandAsync(Controller.Port, $"G0  {string.Join(" ", temp)}");
            }
        }
    }
}
