using CncJs.Api;
using CncJs.Api.Models;
using Microsoft.AspNetCore.Components;

namespace CncJs.Pendant.Web.Shared
{
    public partial class GcodeWidget
    {
        [Inject] public CncJsClient Client { get; set; }
        public Gcode Gcode { get; set; }


        protected override void OnInitialized()
        {
            Gcode = Client.GcodeModule.Gcode;
            Client.GcodeModule.OnLoad += GcodeModule_OnLoad;
            Client.GcodeModule.OnUnLoad += GcodeModule_OnUnLoad;
        }

        private void GcodeModule_OnUnLoad(object sender, EventArgs e)
        {
            Gcode = null;
        }

        private void GcodeModule_OnLoad(object sender, Gcode e)
        {
            Gcode = e;
        }
    }
}
