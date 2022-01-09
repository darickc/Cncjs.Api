using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared.Common
{
    public partial class Loading
    {
        [Parameter] public string Size { get; set; } = "";

        [Parameter]
        public bool Inline { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        public string SpinnerSize { get; set; }

        private readonly Dictionary<string, string> _sizes = new Dictionary<string, string>
        {
            {"xs","15px" },
            {"sm","20px" },
            {"md","30px" },
            {"lg","50px" },
            {"xlg","65px" }
        };

        protected override void OnInitialized()
        {
            SpinnerSize = _sizes.TryGetValue(Size, out var size) ? size : _sizes["xlg"];
        }
    }
}
