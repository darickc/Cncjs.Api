using CncJs.Pendant.Web.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Commands
    {
        [Inject] public IDialogService DialogService { get; set; }
        [Parameter]
        public JoggingModel Jogging { get; set; }

        

        public async Task Command(string cmd)
        {
            switch (cmd)
            {
                case "+":
                    Jogging.Next();
                    break;
                case "-":
                    Jogging.Prev();
                    break;
                case "Open":
                    var options = new DialogOptions
                    {
                         FullScreen = true
                    };
                    var parameters = new DialogParameters { { "Jogging", Jogging } };
                    await DialogService.Show<DistanceDialog>("Distance", parameters,options).Result;
                    StateHasChanged();
                    break;
            }
        }
    }
}
