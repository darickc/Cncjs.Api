using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Sender
    {
        [Inject] public CncJsClient Client { get; set; }
        public SenderStatus Status { get; set; }

        public bool CanPlay => Client.ControllerModule?.ControllerState?.State?.Status?.ActiveState != "Alarm" && 
                               Client.WorkflowModule.State is WorkflowState.Idle or WorkflowState.Paused && 
                               !string.IsNullOrEmpty(Status.Name);
        public bool CanPause => Client.WorkflowModule.State is WorkflowState.Running && !string.IsNullOrEmpty(Status.Name);
        public bool CanStop => Client.WorkflowModule.State is WorkflowState.Paused && !string.IsNullOrEmpty(Status.Name);
        public bool CanClose => Client.WorkflowModule.State is WorkflowState.Idle && !string.IsNullOrEmpty(Status.Name);


        protected override void OnInitialized()
        {
            Status = Client.SenderModule.Status;
            Client.SenderModule.OnStatus += SenderModule_OnStatus;
            Client.WorkflowModule.OnState += WorkflowModule_OnState;
        }

        private async void WorkflowModule_OnState(object sender, string e)
        {
            await InvokeAsync(StateHasChanged);
        }

        private async void SenderModule_OnStatus(object sender, SenderStatus e)
        {
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

    }
}
