using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Sender : IDisposable
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] public CncJsClient Client { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        public SenderStatus Status { get; set; }
        public SenderStatus PreviousStatus { get; set; }

        public bool CanPlay => Client.ControllerModule?.ControllerState?.State?.Status?.ActiveState != "Alarm" && 
                               Client.WorkflowModule.State is WorkflowState.Idle or WorkflowState.Paused && 
                               !string.IsNullOrEmpty(Status.Name);
        public bool CanSelectFile => Client.ControllerModule?.ControllerState?.State?.Status?.ActiveState != "Alarm" &&
                               Client.WorkflowModule.State is WorkflowState.Idle;
        public bool CanPause => Client.WorkflowModule.State is WorkflowState.Running && !string.IsNullOrEmpty(Status.Name);
        public bool CanStop => Client.WorkflowModule.State is WorkflowState.Paused && !string.IsNullOrEmpty(Status.Name);
        public bool CanClose => Client.WorkflowModule.State is WorkflowState.Idle && !string.IsNullOrEmpty(Status.Name);


        protected override void OnInitialized()
        {
            Status = Client.SenderModule.Status;
            Client.SenderModule.OnStatus += SenderModule_OnStatus;
            Client.WorkflowModule.OnState += WorkflowModule_OnState;
            Client.ControllerModule.OnState += ControllerModuleOnOnState;
            Client.FeederModule.OnStatus += FeederModuleOnOnStatus;
        }

        private async void FeederModuleOnOnStatus(object sender, FeederStatus e)
        {
            await InvokeAsync(StateHasChanged);
        }

        private async void ControllerModuleOnOnState(object sender, ControllerState e)
        {
            await InvokeAsync(StateHasChanged);
        }

        private async void WorkflowModule_OnState(object sender, string e)
        {
            if (e == WorkflowState.Idle && Client.GcodeModule.Gcode is { CurrentToolIndex: > 0 })
            {
                Client.GcodeModule.Gcode.CurrentToolIndex = 0;
            }
            await InvokeAsync(StateHasChanged);
        }

        private async void SenderModule_OnStatus(object sender, SenderStatus e)
        {
            if (Status is { Hold: true, HoldReason.Data: "M6" } && PreviousStatus != null && PreviousStatus is not { Hold: true, HoldReason.Data: "M6" } && Client.GcodeModule.Gcode != null)
            {
                Client.GcodeModule.Gcode.CurrentToolIndex++;
            }

            PreviousStatus = Status;
            Status = e;
            await InvokeAsync(StateHasChanged);
        }

        private async Task Start()
        {
            if (Client.WorkflowModule.State is WorkflowState.Paused)
            {
                await Client.WorkflowModule.ResumeAsync();
            }
            else
            {
                await Client.WorkflowModule.StartAsync();
            }
        }

        private async Task Stop()
        {
            await Client.WorkflowModule.StopAsync();
        }

        private async Task Pause()
        {
            await Client.WorkflowModule.PauseAsync();
        }

        private async Task Unload()
        {
            await Client.GcodeModule.UnloadAsync();
        }

        private async Task OpenFiles()
        {
            var options = new DialogOptions
            {
                FullScreen = true
            };
            // var parameters = new DialogParameters { { "Jogging", Jogging } };
            var result = await DialogService.Show<FilesDialog>("Files", options).Result;
            if (!result.Cancelled)
            {
                if (result.Data is WatchFile file)
                {
                    await Client.WatchModule.LoadFile(file);
                }
            }
        }

        public void Dispose()
        {
            Client.SenderModule.OnStatus -= SenderModule_OnStatus;
            Client.WorkflowModule.OnState -= WorkflowModule_OnState;
            Client.ControllerModule.OnState -= ControllerModuleOnOnState;
            Client.FeederModule.OnStatus += FeederModuleOnOnStatus;
        }
    }
}
