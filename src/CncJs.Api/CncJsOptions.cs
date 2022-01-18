namespace CncJs.Api;

public class CncJsOptions
{
    public string Secret { get; set; }
    public string Name { get; set; } = "CncJs.Api";
    public int SocketPort { get; set; } = 8080;
    public string SocketAddress { get; set; } = "localhost";
    public int AccessTokenLifetime { get; set; } = 30;
    public bool SecureConnection { get; set; }
    public string Token { get; set; }

    private string Port => SocketPort != 80 ? $":{SocketPort}" : "";
    private string WebSocketProtocol => SecureConnection ? "wss" : "ws";
    private string ApiProtocol => SecureConnection ? "https" : "http";

    public string WebSocketUrl=> $"{WebSocketProtocol}://{SocketAddress}{Port}";
    public string ApiUrl => $"{ApiProtocol}://{SocketAddress}{Port}/api/";
}