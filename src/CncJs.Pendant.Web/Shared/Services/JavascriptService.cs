using CncJs.Api.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CncJs.Pendant.Web.Shared.Services;

public class JavascriptService
{
    private readonly IJSRuntime _jsRuntime;
    private          bool       _initialized;

    public event EventHandler<KeyboardEventArgs> KeyUp;
    public event EventHandler<KeyboardEventArgs> KeyDown;

    public JavascriptService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task Initialize()
    {
        if (!_initialized)
        {
            _initialized = true;
            await _jsRuntime.InvokeVoidAsync("setJogger", DotNetObjectReference.Create(this));
        }
    }

    [JSInvokable]
    public void OnKeyDown(KeyboardEventArgs args)
    {
        KeyDown?.Invoke(this, args);
    }

    [JSInvokable]
    public void OnKeyUp(KeyboardEventArgs args)
    {
        KeyUp?.Invoke(this, args);
    }

    public async Task<bool> IsTouchScreen()
    {
        return await _jsRuntime.InvokeAsync<bool>("isTouchScreen");
    }

    public async Task SetupToolPath()
    {
        await _jsRuntime.InvokeVoidAsync("toolpathSetup");
    }

    public async Task ShowToolPath(string gcode, Modal modal, Position wpos, Position mpos)
    {
        await _jsRuntime.InvokeVoidAsync("displayer.showToolpath", gcode, modal, wpos, mpos);
    }

    public async Task ReDrawToolPath(Modal modal, Position mpos)
    {
        await _jsRuntime.InvokeVoidAsync("displayer.reDrawTool", modal, mpos);
    }
}