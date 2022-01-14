namespace CncJs.Api.Models;

public class ControllerSettings
{
    private Settings _settings;
    private string   _type;

    public string Type
    {
        get => _type;
        set
        {
            _type = value;
            Update();
        }
    }

    public Settings Settings
    {
        get => _settings;
        set
        {
            _settings = value;
            Update();
        }
    }

    public Units MachineUnits { get; private set; }
    public double MaxFeedrate { get; private set; }

    private void Update()
    {
        if (string.IsNullOrEmpty(_type) || _settings == null)
        {
            return;
        }
        UpdateMachineUnits();
        UpdateMaxFeedrate();
    }

    private void UpdateMachineUnits()
    {
        if (Type == ControllerTypes.Grbl)
        {
            MachineUnits =(Units) (_settings.GetSetting("$110").ToInt() ?? 0);
        }
    }

    private void UpdateMaxFeedrate()
    {
        var maxFeedrates = new List<double?>
        {
            _settings.GetSetting("$110").ToDouble(), // x
            _settings.GetSetting("$111").ToDouble(), // y
            _settings.GetSetting("$112").ToDouble()  // z
        };
        MaxFeedrate = maxFeedrates.Where(f => f.HasValue).Max() ?? 0;
    }


}