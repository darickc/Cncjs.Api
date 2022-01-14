using CncJs.Api.Models;

namespace CncJs.Pendant.Web.Models;

public class JoggingModel
{
    private readonly string[] _invalidStates = { "Hold", "Run", "Alarm" };
    private          int      _index;
    private          Units    _units = Units.Metric;
    private List<double> _metricDistances => new()
    {
        .001,
        .002,
        .003,
        .005,
        .01,
        .02,
        .03,
        .05,
        .1,
        .2,
        .3,
        .5,
        1,
        2,
        3,
        5,
        10,
        20,
        30,
        50,
        100,
        200,
        300,
        500
    };

    private List<double> _imperialDistances => new()
    {
        .0001,
        .0002,
        .0003,
        .0005,
        .001,
        .002,
        .003,
        .005,
        .01,
        .02,
        .03,
        .05,
        .1,
        .2,
        .3,
        .5,
        1,
        2,
        3,
        5,
        10,
        20
    };

    public double Distance { get; set; } = 1;
    public List<double> Distances { get; private set; }

    public Units Units
    {
        get => _units;
        set
        {
            if (_units == value)
            {
                return;
            }
            _units = value;
            Distances = _units == Units.Metric ? _metricDistances : _imperialDistances;
            if (_index >= Distances.Count)
            {
                _index = Distances.Count - 1;
            }
            Distance = Distances[_index];
        }
    }

    public bool CanIncrease => (_index + 1) < Distances.Count;
    public bool CanDecrease => _index > 0;

    public JoggingModel()
    {
        Distances = _metricDistances;
        _index = Distances.IndexOf(Distance);
    }

    public void Next()
    {
        if (CanIncrease)
        {
            Distance = Distances[++_index];
        }
    }

    public void Prev()
    {
        if (CanDecrease)
        {
            Distance = Distances[--_index];
        }
    }

    public void Set(double d)
    {
        Distance = d;
        if (Distances.Contains(Distance))
        {
            _index = Distances.IndexOf(Distance);
        }
    }

    public bool CanJog(string activeState)
    {
        return !_invalidStates.Contains(activeState);
    }
}