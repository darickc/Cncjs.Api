using CncJs.Api;
using CncJs.Api.Models;

namespace CncJs.Pendant.Web.Models;

public class FeedrateModel
{
    private          Units        _units = Units.Metric;
    private          int          _index;
    private readonly List<double> _metricFeedrates;
    private readonly List<double> _imperialFeedrates;
    public double Feedrate { get; set; } = 1;
    public List<double> Feedrates { get; private set; }

    public bool CanIncrease => _index+1 < Feedrates.Count;
    public bool CanDecrease => _index > 0;

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
            Feedrates = _units == Units.Metric ? _metricFeedrates : _imperialFeedrates;
            if (_index >= Feedrates.Count)
            {
                _index = Feedrates.Count - 1;
            }
            Feedrate = Feedrates[_index];
        }
    }

    

    public FeedrateModel(double maxFeedrate, Units machineUnits, Units currentUnits)
    {
        if (machineUnits == Units.Metric)
        {
            _metricFeedrates = CreateFeedrates(maxFeedrate, 250, 50, 100, 500);
            maxFeedrate = Math.Round(maxFeedrate.ToImperial(), MidpointRounding.ToZero) ;
            _imperialFeedrates = CreateFeedrates(maxFeedrate, 10, 2, 4, 20);
            Feedrates = _metricFeedrates;
            Feedrate = 1000;
        }
        else
        {
            _imperialFeedrates = CreateFeedrates(maxFeedrate, 10, 2, 4, 20);
            maxFeedrate = Math.Round(maxFeedrate.ToMetric(), MidpointRounding.ToZero);
            _metricFeedrates = CreateFeedrates(maxFeedrate, 250, 50, 100, 500);
            Feedrates = _imperialFeedrates;
            Feedrate = 40;
        }
        
        Feedrate = Feedrates.Contains(Feedrate) ? Feedrate : Feedrates.Last();
        _index = Feedrates.IndexOf(Feedrate);
        Units = currentUnits;
    }

    private List<double> CreateFeedrates(double maxFeedrate, double interval, params double[] initialRates)
    {
        var rates = initialRates.ToList();
        double temp = rates.Last() + interval;
        while (temp < maxFeedrate)
        {
            rates.Add(temp);
            temp += interval;
        }

        if (!rates.Contains(maxFeedrate))
        {
            rates.Add(maxFeedrate);
        }
        return rates;
    }

    public void Next()
    {
        if (CanIncrease)
        {
            Feedrate = Feedrates[++_index];
        }
    }

    public void Prev()
    {
        if (CanDecrease)
        {
            Feedrate = Feedrates[--_index];
        }
    }

    public void Set(double d)
    {
        Feedrate = d;
        if (Feedrates.Contains(Feedrate))
        {
            _index = Feedrates.IndexOf(Feedrate);
        }
    }
}