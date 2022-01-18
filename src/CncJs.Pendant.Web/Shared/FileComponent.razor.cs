using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Shared
{
    public partial class FileComponent
    {
        private readonly string[] _suffixes =
            { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        [Parameter] public WatchFile File { get; set; }
        [Parameter] public EventCallback<WatchFile> OnClick { get; set; }
        [Parameter] public int Level { get; set; }


        private string GetIcon(WatchFile file)
        {
            return file.FileType == WatchFileType.Directory
                ? Icons.Custom.Uncategorized.Folder
                : Icons.Custom.FileFormats.FileCode;
        }

        private async Task Select(WatchFile file)
        {
            await OnClick.InvokeAsync(file);
        }


        // Load all suffixes in an array  

        private string FormatSize(int bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return $"{number:n1}{_suffixes[counter]}";
        }


    }
}
