﻿using Cncjs.Api;
using Cncjs.Api.Models;
using CncJs.Pendant.Web.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CncJs.Pendant.Web.Pages
{
    public partial class Index
    {
        [Inject] public CncJsClient Client { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        public ControllerModel Controller { get; set; }
        public ControllerState ControllerState { get; set; }

        public bool PortOpened => Controller != null;
        public bool ShowPorts { get; set; } = true;
        public JoggingModel Jogging { get; set; } = new();


        protected override void OnInitialized()
        {
            Client.OnDisconnected = OnDisconnected;
            Client.SerialPort.OnOpen = OnOpen;
            Client.SerialPort.OnClose = OnClose;
            Client.OnError = OnError;
            Client.Controller.OnState = OnState;
        }

        private void OnState(ControllerState obj)
        {
            ControllerState = obj;
            InvokeAsync(StateHasChanged);
        }

        private void OnError(string e)
        {
            InvokeAsync(()=> Snackbar.Add(e, Severity.Error));
        }

        private Task OnClose(ControllerModel arg)
        {
            Controller = null;
            InvokeAsync(StateHasChanged);
            return Task.CompletedTask;
        }

        private Task OnOpen(ControllerModel controller)
        {
            Controller = controller;
            ShowPorts = false;
            InvokeAsync(StateHasChanged);
            return Task.CompletedTask;
        }

        private async Task OnDisconnected()
        {
            await Client.ConnectAsync();
        }
    }
}
