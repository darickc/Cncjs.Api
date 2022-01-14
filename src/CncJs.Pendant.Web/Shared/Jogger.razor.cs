using CncJs.Api;
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
        public JoggingModel Jogging { get; set; }
        [Parameter]
        public FeedrateModel Feedrate { get; set; }

        public Timer Timer { get; set; }

        public bool IsTouchscreen { get; set; }


        public bool Disabled => Client.ControllerModule.ControllerState?.State?.Status?.ActiveState == "Alarm";

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
            IsTouchscreen = await KeyboardService.IsTouchScreen();
        }
        
        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await Client.GcodeModule.JogAsync( _currentValue, 1000, Feedrate.Feedrate);
            Timer.Enabled = false;
        }

        private void Jog(string value, bool touch= false)
        {
            if (IsTouchscreen && !touch)
            {
                return;
            }
            _currentValue = value;
            Timer.Start();
        }

        private async Task CancelJog(bool touch = false)
        {
            if (IsTouchscreen && !touch)
            {
                return;
            }
            if (Timer.Enabled)
            {
                Timer.Stop();
                await Client.GcodeModule.JogAsync( _currentValue, Jogging.Distance, Feedrate.Feedrate);
            }
            else
            {
                await StopJogging();
            }
        }

        private async Task StopJogging()
        {
            await Client.GcodeModule.CancelJogAsync();
        }

        public async void OnKeyDown(object sender, KeyboardEventArgs args)
        {
            Logger.LogInformation($"keydown: {args.Key}:{args.Code}");
            if (!args.Repeat)
            {
                switch (args.Code)
                {
                    case "Numpad1":
                        Jog("X-Y-", true);
                        break;
                    case "Numpad2":
                        Jog("Y-", true);
                        break;
                    case "Numpad3":
                        Jog("X+Y-", true);
                        break;
                    case "Numpad4":
                        Jog("X-", true);
                        break;
                    case "Numpad5":
                        await StopJogging();
                        break;
                    case "Numpad6":
                        Jog("X+", true);
                        break;
                    case "Numpad7":
                        Jog("X-Y+", true);
                        break;
                    case "Numpad8":
                        Jog("Y+", true);
                        break;
                    case "Numpad9":
                        Jog("X+Y+", true);
                        break;
                    case "NumpadAdd":
                        Jog("Z+", true);
                        break;
                    case "NumpadEnter":
                        Jog("Z-", true);
                        break;
                }

            }
        }

        public async void OnKeyUp(object sender, KeyboardEventArgs args)
        {
            Logger.LogInformation($"keyup: {args.Key}:{args.Code}");
            if (!args.Repeat)
            {
                switch (args.Code)
                {
                    case "Numpad1":
                    case "Numpad2":
                    case "Numpad3":
                    case "Numpad4":
                    case "Numpad6":
                    case "Numpad7":
                    case "Numpad8":
                    case "Numpad9":
                    case "NumpadAdd":
                    case "NumpadEnter":
                        await CancelJog(true);
                        break;
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
