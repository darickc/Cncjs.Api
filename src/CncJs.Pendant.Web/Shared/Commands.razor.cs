using Cncjs.Api;
using Cncjs.Api.Models;
using CncJs.Pendant.Web.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Commands
    {
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public CncJsClient Client { get; set; }
        [Parameter]
        public JoggingModel Jogging { get; set; }
        [Parameter]
        public ControllerState ControllerState { get; set; }
        [Parameter]
        public ControllerModel Controller { get; set; }

        public bool Disabled => ControllerState?.State?.Status?.ActiveState == "Alarm";


        public async Task Command(string cmd)
        {
            switch (cmd)
            {
                case "+":
                    Jogging.Next();
                    break;
                case "-":
                    Jogging.Prev();
                    break;
                case "Open":
                    var options = new DialogOptions
                    {
                         FullScreen = true
                    };
                    var parameters = new DialogParameters { { "Jogging", Jogging } };
                    await DialogService.Show<DistanceDialog>("Distance", parameters,options).Result;
                    StateHasChanged();
                    break;
                case "Unlock":
                    await Client.Controller.UnlockAsync(Controller.Port);
                    break;
                case "Reset":
                    await Client.Controller.ResetAsync(Controller.Port);
                    break;
                case "Home":
                    await Client.Controller.HomeAsync(Controller.Port);
                    break;
                case "0X":
                case "0Y":
                case "0X0Y":
                    await SetZero(cmd);
                    break;
            }
        }

        private async Task SetZero(string cmd)
        {
            var letters = new[] { "X", "Y", "Z" };
            var workspace = ControllerState.State.Parserstate.Modal.Workspace;
            var temp = new List<string>();
            foreach (var letter in letters)
            {
                if (cmd.Contains($"{letter}"))
                {
                    temp.Add($"{letter}0");
                }
            }
            await Client.Gcode.SendCommandAsync(Controller.Port, $"G10 L20 {workspace} {string.Join("", temp)}");
        }
    }
}
