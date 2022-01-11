using Cncjs.Api;
using Cncjs.Api.Models;
using CncJs.Pendant.Web.Models;
using CncJs.Pendant.Web.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Timer = System.Timers.Timer;

namespace CncJs.Pendant.Web.Shared
{
    public partial class Jogger : IDisposable
    {
        [Inject] KeyboardService KeyboardService { get; set; }
        [Inject] public CncJsClient Client { get; set; }
        [Inject] public ILogger<Jogger> Logger { get; set; }
        [Parameter]
        public ControllerState ControllerState { get; set; }
        [Parameter]
        public ControllerModel Controller { get; set; }

        public Timer Timer { get; set; }

        [Parameter]
        public JoggingModel Jogging { get; set; }

        public bool Disabled => ControllerState?.State?.Status?.ActiveState == "Alarm";

        private string                        _currentValue;

        public Jogger()
        {
            Timer = new Timer(250);
            Timer.Elapsed += Timer_Elapsed;
        }

        protected override async Task OnInitializedAsync()
        {
            KeyboardService.OnKeyDown += OnKeyDown;
            KeyboardService.OnKeyUp += OnKeyUp;
            await KeyboardService.Initialize();
        }
        
        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await Client.Gcode.JogAsync(Controller.Port, _currentValue, 1000, 1000);
            Timer.Enabled = false;
        }

        private void Jog(string value)
        {
            if (ControllerState == null)
            {
                return;
            }

            _currentValue = value;
            Timer.Start();
        }

        private async Task CancelJog()
        {
            if (Timer.Enabled)
            {
                Timer.Stop();
                await Client.Gcode.JogAsync(Controller.Port, _currentValue, Jogging.Distance, 1000);
            }
            else
            {
                await StopJogging();
            }
        }

        private async Task StopJogging()
        {
            if (ControllerState.State.Status.ActiveState == "Jog")
                await Client.Controller.FeedholdAsync(Controller.Port);
        }

        public async void OnKeyDown(object sender, KeyboardEventArgs args)
        {
            Logger.LogInformation($"keydown: {args.Key}:{args.Code}");
            if (!args.Repeat)
            {
                switch (args.Code)
                {
                    case "Numpad1":
                        Jog("X-Y-");
                        break;
                    case "Numpad2":
                        Jog("Y-");
                        break;
                    case "Numpad3":
                        Jog("X+Y-");
                        break;
                    case "Numpad4":
                        Jog("X-");
                        break;
                    case "Numpad5":
                        await StopJogging();
                        break;
                    case "Numpad6":
                        Jog("X+");
                        break;
                    case "Numpad7":
                        Jog("X-Y+");
                        break;
                    case "Numpad8":
                        Jog("Y+");
                        break;
                    case "Numpad9":
                        Jog("X+Y+");
                        break;
                    case "NumpadAdd":
                        Jog("Z+");
                        break;
                    case "NumpadEnter":
                        Jog("Z-");
                        break;
                }

            }
        }

        public async void OnKeyUp(object sender, KeyboardEventArgs args)
        {
            Logger.LogInformation($"keyup: {args.Key}:{args.Code}");
            if (!args.Repeat)
            {
                if (args.Code != "Numpad5")
                {
                    await CancelJog();
                }
            }
        }

        public void Dispose()
        {
            Timer?.Dispose();
            KeyboardService.OnKeyDown -= OnKeyDown;
            KeyboardService.OnKeyUp -= OnKeyUp;
        }
    }
}
