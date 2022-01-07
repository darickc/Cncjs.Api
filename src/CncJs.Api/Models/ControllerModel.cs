namespace Cncjs.Api.Models;

public class ControllerModel
{
    public string Port { get; set; }
    public int Baudrate { get; set; } = 115200;
    public string ControllerType { get; set; } = ControllerTypes.Grbl;
    public bool InUse { get; set; }

    public ControllerModel()
    {
        
    }
    public ControllerModel(string port, string controllerType, int? baudrate = null)
    {
        Port = port;
        ControllerType = controllerType;
        Baudrate = baudrate ?? Baudrate;
    }
}