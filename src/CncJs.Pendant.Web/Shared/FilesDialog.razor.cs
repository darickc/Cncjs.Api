using CncJs.Api;
using CncJs.Api.Models;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class FilesDialog
    {
        [Inject] public CncJsClient Client { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        public WatchFile SelectedFile { get; set; }

        void Close() => MudDialog.Close(DialogResult.Ok(SelectedFile));
        void Cancel() => MudDialog.Close(DialogResult.Cancel());

        protected override async Task OnInitializedAsync()
        {
            // Files = Client.WatchModule.Files;
            if (Client.WatchModule.Files == null)
            {
                await Client.WatchModule.GetFiles();
                StateHasChanged();
            }
            else
            {
                UnselectFiles(Client.WatchModule.Files);
            }
        }

        private void UnselectFiles(WatchFile[] files)
        {
            foreach (var file in files)
            {
                if (file.Selected)
                {
                    SelectedFile = file;
                    return;
                }
                if (file.Files != null)
                {
                    UnselectFiles(file.Files);
                }
            }
        }
        
        private async Task Select(WatchFile file)
        {
            if (file.FileType == WatchFileType.Directory)
            {
                if (file.Expanded)
                {
                    file.Expanded = false;
                }
                else
                {
                    if (file.Files == null)
                    {
                        file.Loading = true;
                        await Client.WatchModule.GetFiles(file.Path)
                            .Tap(files => file.Files = files)
                            .Finally(_=> file.Loading = false);
                    }

                    file.Expanded = true;
                    StateHasChanged();
                }
            }
            else
            {
                if (SelectedFile != null)
                {
                    SelectedFile.Selected = false;
                }
                SelectedFile = file;
                file.Selected = true;
            }
        }

        private async Task Refresh()
        {
            await Client.WatchModule.GetFiles();
            StateHasChanged();
        }
    }
}
