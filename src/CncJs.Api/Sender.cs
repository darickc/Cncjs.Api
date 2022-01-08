﻿using Microsoft.Extensions.Logging;
using SocketIOClient;

namespace Cncjs.Api;

public class Sender
{
    private readonly CncJsSocketIo _client;
    private const    string        Status = "sender:status";
    public Action OnStatus { get; set; }
    internal Sender(CncJsSocketIo client)
    {
        _client = client;
        _client.On(Status, OnStatusEvent);
    }
    private void OnStatusEvent(SocketIOResponse obj)
    {
        OnStatus?.Invoke();
    }

}