﻿using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CncJs.Pendant.Web.Shared.Services;

public class KeyboardService
{
    private readonly IJSRuntime _jsRuntime;
    private          bool       _initialized;

    public event EventHandler<KeyboardEventArgs> OnKeyUp;
    public event EventHandler<KeyboardEventArgs> OnKeyDown;

    public KeyboardService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task Initialize()
    {
        _initialized = true;
        await _jsRuntime.InvokeVoidAsync("setJogger", DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public async Task KeyDown(KeyboardEventArgs args)
    {
        OnKeyDown?.Invoke(this, args);
    }

    [JSInvokable]
    public async Task KeyUp(KeyboardEventArgs args)
    {
        OnKeyUp?.Invoke(this, args);
    }
}