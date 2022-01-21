using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class CommandsDialog
    {
        [Inject] public CncJsClient Client { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public Command[] Commands { get; set; }
        public Command Command { get; set; }
        
        void Close() => MudDialog.Close(DialogResult.Ok(Command));
        void Cancel() => MudDialog.Close(DialogResult.Cancel());

        private void SelectCommand(Command c)
        {
            Command = c;
        }
    }
}
