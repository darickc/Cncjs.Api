using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class MacrosDialog
    {
        [Inject] public CncJsClient Client { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public Macro[] Macros { get; set; }

        public Macro Macro { get; set; }
        void Close() => MudDialog.Close(DialogResult.Ok(Macro));
        void Cancel() => MudDialog.Close(DialogResult.Cancel());
        private void SelectMacro(Macro c)
        {
            Macro = c;
        }
    }
}
