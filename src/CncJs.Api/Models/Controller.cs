namespace CncJs.Api.Models;

public class Controller
{
    public string Port { get; set; }
    public int Baudrate { get; set; } = 115200;
    public string ControllerType { get; set; } = ControllerTypes.Grbl;
    public bool InUse { get; set; }

    public Controller()
    {
        
    }
    public Controller(string port, string controllerType, int? baudrate = null)
    {
        Port = port;
        ControllerType = controllerType;
        Baudrate = baudrate ?? Baudrate;
    }
}