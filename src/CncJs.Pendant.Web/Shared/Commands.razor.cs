using CncJs.Api;
using CncJs.Api.Models;
using CncJs.Pendant.Web.Models;
using CncJs.Pendant.Web.Shared.Services;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Commands : IDisposable
    {
        [Inject] public ILogger<Commands> Logger { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] KeyboardService KeyboardService { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public CncJsClient Client { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Parameter]
        public JoggingModel Jogging { get; set; }
        [Parameter]
        public FeedrateModel Feedrate { get; set; }


        public Macro[] Macros { get; set; } = Array.Empty<Macro>();
        public Command[] CommandItems { get; set; } = Array.Empty<Command>();

        public bool Disabled => !Jogging?.CanJog(Client.ControllerModule.ControllerState?.State?.Status?.ActiveState) ?? true;

        protected override async Task OnInitializedAsync()
        {
            KeyboardService.OnKeyUp += OnKeyUp;
            await KeyboardService.Initialize();
            await Client.MacroModule.GetMacros()
                .Tap(macros => Macros = macros)
                .OnFailure(e=> Snackbar.Add(e, Severity.Error));
            await Client.CommandModule.GetCommands()
                .Tap(commands => CommandItems = commands)
                .OnFailure(e => Snackbar.Add(e, Severity.Error));
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
                    Feedrate?.Next();
                    break;
                case "F-":
                    Feedrate?.Prev();
                    break;
                case "FOpen":
                    var parameters2 = new DialogParameters { { "Feedrate", Feedrate } };
                    await DialogService.Show<FeedrateDialog>("Feedrate", parameters2, options).Result;
                    StateHasChanged();
                    break;
                case "Unlock":
                    await Client.ControllerModule.UnlockAsync();
                    break;
                case "Reset":
                    await Client.ControllerModule.ResetAsync();
                    break;
                case "Home":
                    await Client.ControllerModule.HomeAsync();
                    break;
                case "Macros":
                    var parameters3 = new DialogParameters { { "Macros", Macros } };
                    var result2 = await DialogService.Show<MacrosDialog>("Macros", parameters3, options).Result;
                    if (!result2.Cancelled)
                    {
                        if (result2.Data is Macro macro)
                        {
                            await RunMacro(macro.Id);
                        }
                    }
                    StateHasChanged();
                    break;
                case "Commands":
                    var parameters4 = new DialogParameters { { "Commands", CommandItems } };
                    var result = await DialogService.Show<CommandsDialog>("Commands", parameters4, options).Result;
                    if (!result.Cancelled)
                    {
                        if (result.Data is Command command)
                        {
                            await Client.CommandModule.Run(command.Id);
                        }
                    }
                    StateHasChanged();
                    break;
            }
        }

        private async Task SetZero(string cmd)
        {
            var workspace = Client.ControllerModule.ControllerState.State.Parserstate.Modal.Workspace;
            await Client.GcodeModule.SetZeroAsync( workspace,cmd);
        }

        private async Task Jog(string value)
        {
            await Client.GcodeModule.JogAsync( value, Jogging.Distance, Feedrate.Feedrate);
        }

        private async Task RunMacro(string id)
        {
            if (Macros?.Any(m=>m.Id == id) != true)
            {
                return;
            }
            var boundingBox = await GetBoundingBox();

            var context = new Context
            {
                Xmax = boundingBox.Max.X,
                Xmin = boundingBox.Min.X,
                Ymax = boundingBox.Max.Y,
                Ymin = boundingBox.Min.Y
            };
            await Client.MacroModule.RunMacro(id, context);
        }

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
                case "NumpadDivide":
                    await Command("-");
                    break;
            }
            StateHasChanged();
        }

        private async Task<BoundingBox> GetBoundingBox()
        {
            return await JsRuntime.InvokeAsync<BoundingBox>("getBoundingBox");
        }

        public void Dispose()
        {
            KeyboardService.OnKeyUp -= OnKeyUp;
        }
    }
}
