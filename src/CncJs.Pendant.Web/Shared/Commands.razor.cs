using Cncjs.Api;
using Cncjs.Api.Models;
using CncJs.Pendant.Web.Models;
using CncJs.Pendant.Web.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Commands : IDisposable
    {
        [Inject] KeyboardService KeyboardService { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public CncJsClient Client { get; set; }
        [Parameter]
        public JoggingModel Jogging { get; set; }
        [Parameter]
        public ControllerState ControllerState { get; set; }
        [Parameter]
        public ControllerModel Controller { get; set; }

        public bool Disabled => ControllerState?.State?.Status?.ActiveState == "Alarm";

        protected override async Task OnInitializedAsync()
        {
            KeyboardService.OnKeyDown += OnKeyDown;
            KeyboardService.OnKeyUp += OnKeyUp;
            await KeyboardService.Initialize();
        }

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
            }
        }

        private async Task SetZero(string cmd)
        {
            var workspace = ControllerState.State.Parserstate.Modal.Workspace;
            await Client.Gcode.SetZeroAsync(Controller.Port, workspace,cmd);
        }

        private async Task Jog(string value)
        {
            if (ControllerState == null)
            {
                return;
            }

            await Client.Gcode.JogAsync(Controller.Port, value, Jogging.Distance, 1000);
        }

        public async void OnKeyDown(object sender, KeyboardEventArgs args){}

        public async void OnKeyUp(object sender, KeyboardEventArgs args)
        {
            switch (args.Code)
            {
                case "Numpad0":
                    await Jog("X0Y0");
                    break;
                case "NumpadDecimal":
                    await SetZero("XY");
                    break;
                case "NumpadMultiply":
                    await Command("+");
                    break;
                case "NumpadSubtract":
                    await Command("-");
                    break;
            }
            StateHasChanged();
        }

        public void Dispose()
        {
            KeyboardService.OnKeyDown -= OnKeyDown;
            KeyboardService.OnKeyUp -= OnKeyUp;
        }
    }
}
