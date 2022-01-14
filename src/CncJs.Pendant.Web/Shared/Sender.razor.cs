using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Sender
    {
        [Inject] public CncJsClient Client { get; set; }
        public SenderStatus Status { get; set; }

        protected override void OnInitialized()
        {
            Status = Client.SenderModule.Status;
            Client.SenderModule.OnStatus += SenderModule_OnStatus;
        }

        private async void SenderModule_OnStatus(object sender, SenderStatus e)
        {
            Status = e;
            await InvokeAsync(StateHasChanged);
        }
    }
}
