using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CncJs.Pendant.Web.Shared
{
    public partial class ToolpathViewer
    {
        [Inject] public CncJsClient Client { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        private bool _rendered;

        private Gcode _gcode;

        protected override async Task OnInitializedAsync()
        {
            Client.ControllerModule.OnState += ControllerModuleOnOnState;
            Client.GcodeModule.OnLoad += GcodeModule_OnLoad;
            if (Client.GcodeModule.Gcode != null)
            {
                await LoadGcode();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _rendered = true;
            await LoadGcode();
        }

        private async void GcodeModule_OnLoad(object sender, Gcode e)
        {
            await LoadGcode();
        }

        private async Task LoadGcode()
        {
            if (_rendered && Client.GcodeModule.Gcode != null && _gcode?.FileName != Client.GcodeModule.Gcode.FileName && Client.ControllerModule.ControllerState != null)
            {
                await JsRuntime.InvokeVoidAsync("showToolpath", Client.GcodeModule.Gcode.Code, Client.ControllerModule.ControllerState.State.Status.Wpos, Client.ControllerModule.ControllerState.State.Status.Mpos);
                _gcode = Client.GcodeModule.Gcode;
            }
        }

        private async void ControllerModuleOnOnState(object sender, ControllerState e)
        {
            await LoadGcode();
        }
    }
}
