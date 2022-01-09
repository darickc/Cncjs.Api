using CncJs.Pendant.Web.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class DistanceDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public JoggingModel Jogging { get; set; }
        void Close() => MudDialog.Close(DialogResult.Ok(true));

        public void SelectDistance(double d)
        {
            Jogging.Set(d);
            Close();
        }
    }
}
