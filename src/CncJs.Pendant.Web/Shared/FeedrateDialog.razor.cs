using CncJs.Pendant.Web.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class FeedrateDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public FeedrateModel Feedrate { get; set; }

        void Close() => MudDialog.Close(DialogResult.Ok(true));

        public void SelectFeedrate(double d)
        {
            Feedrate.Set(d);
            Close();
        }
    }
}
