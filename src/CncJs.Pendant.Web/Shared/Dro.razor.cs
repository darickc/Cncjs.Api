using Cncjs.Api;
using Cncjs.Api.Models;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Dro
    {
        [Parameter]
        public ControllerState ControllerState { get; set; }

        public State State => ControllerState?.State;

    }
}
