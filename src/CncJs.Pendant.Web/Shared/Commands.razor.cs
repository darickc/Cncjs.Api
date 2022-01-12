using Cncjs.Api;
using Cncjs.Api.Models;
using CncJs.Pendant.Web.Models;
using CncJs.Pendant.Web.Shared.Services;
using CSharpFunctionalExtensions;
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
        [Inject] ISnackbar Snackbar { get; set; }
        [Parameter]
        public JoggingModel Jogging { get; set; }
        [Parameter]
        public FeedrateModel Feedrate { get; set; }
        [Parameter]
        public ControllerState ControllerState { get; set; }
        [Parameter]
        public ControllerModel Controller { get; set; }

        public MacroModel[] Macros { get; set; }

        public bool Disabled => ControllerState?.State?.Status?.ActiveState == "Alarm";

        protected override async Task OnInitializedAsync()
        {
            KeyboardService.OnKeyDown += OnKeyDown;
            KeyboardService.OnKeyUp += OnKeyUp;
            await KeyboardService.Initialize();
            await Client.Macro.GetMacros()
                .Tap(macros => Macros = macros)
                .OnFailure(e=> Snackbar.Add(e, Severity.Error));
            StateHasChanged();
        }

        public async Task Command(string cmd)
        {
            var options = new DialogOptions
            {
                FullScreen = true
            };
            switch (cmd)
            {
                case "+":
                    Jogging.Next();
                    break;
                case "-":
                    Jogging.Prev();
                    break;
                case "Open":
                    var parameters = new DialogParameters { { "Jogging", Jogging } };
                    await DialogService.Show<DistanceDialog>("Distance", parameters,options).Result;
                    StateHasChanged();
                    break;
                case "F+":
                    Feedrate.Next();
                    break;
                case "F-":
                    Feedrate.Prev();
                    break;
                case "FOpen":
                    var parameters2 = new DialogParameters { { "Feedrate", Feedrate } };
                    await DialogService.Show<FeedrateDialog>("Feedrate", parameters2, options).Result;
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

            await Client.Gcode.JogAsync(Controller.Port, value, Jogging.Distance, Feedrate.Feedrate);
        }

        private async Task RunMacro(string id)
        {
            if (Macros?.Any(m=>m.Id == id) != true)
            {
                return;
            }

            await Client.Macro.RunMacro(Controller.Port, id);
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
