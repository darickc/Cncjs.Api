using CncJs.Api;
using CncJs.Api.Models;
using CncJs.Pendant.Web.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class ToolpathViewer
    {
        [Inject] public CncJsClient Client { get; set; }
        [Inject] public JavascriptService JavascriptService { get; set; }

        private bool _rendered;
        private Gcode _gcode;

        protected override async Task OnInitializedAsync()
        {
            Client.ControllerModule.OnState += ControllerModuleOnState;
            Client.GcodeModule.OnLoad += GcodeModule_OnLoad;
            if (Client.GcodeModule.Gcode != null)
            {
                await LoadGcode();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _rendered = true;
            if (firstRender)
            {
                // await JsRuntime.InvokeVoidAsync("toolpathSetup");
                await JavascriptService.SetupToolPath();
                await LoadGcode();
            }
        }

        private async void GcodeModule_OnLoad(object sender, Gcode e)
        {
            await LoadGcode();
        }

        private async Task LoadGcode()
        {
            if (_rendered && Client.GcodeModule.Gcode != null && Client.ControllerModule.ControllerState != null)
            {

                try
                {
                    if (_gcode?.FileName != Client.GcodeModule.Gcode.FileName)
                    {
                        // await JsRuntime.InvokeVoidAsync("displayer.showToolpath", Client.GcodeModule.Gcode.Code, Client.ControllerModule.ControllerState.State.Parserstate.Modal, Client.ControllerModule.ControllerState.State.Status.Wpos, Client.ControllerModule.ControllerState.State.Status.Mpos);
                        await JavascriptService.ShowToolPath(Client.GcodeModule.Gcode.Code,
                            Client.ControllerModule.ControllerState.State.Parserstate.Modal,
                            Client.ControllerModule.ControllerState.State.Status.Wpos,
                            Client.ControllerModule.ControllerState.State.Status.Mpos);
                        _gcode = Client.GcodeModule.Gcode;
                    }
                    else
                    {
                        // await JsRuntime.InvokeVoidAsync("displayer.reDrawTool", Client.ControllerModule.ControllerState.State.Parserstate.Modal, Client.ControllerModule.ControllerState.State.Status.Mpos);
                        await JavascriptService.ReDrawToolPath(
                            Client.ControllerModule.ControllerState.State.Parserstate.Modal,
                            Client.ControllerModule.ControllerState.State.Status.Mpos);
                    }
                }
                catch
                {
                }
            }
        }

        private async void ControllerModuleOnState(object sender, ControllerState e)
        {
            await LoadGcode();
        }
    }
}
