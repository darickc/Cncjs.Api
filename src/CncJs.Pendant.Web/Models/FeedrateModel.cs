namespace CncJs.Pendant.Web.Models;

public class FeedrateModel
{
    public double Feedrate { get; set; } = 1;
    public List<double> Feedrates => new ()
    {
        50,100, 500
    };

    public bool CanIncrease => _index < Feedrates.Count;
    public bool CanDecrease => _index > 0;

    private int _index;

    public FeedrateModel(double maxFeedrate)
    {
        int temp = 1000;
        while (temp < maxFeedrate)
        {
            Feedrates.Add(temp);
            temp += 500;
        }

        Feedrate = Feedrates.Contains(1500) ? 1500 : Feedrates.Last();
        _index = Feedrates.IndexOf(Feedrate);
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

    public void Set(int d)
    {
        Feedrate = d;
        if (Feedrates.Contains(Feedrate))
        {
            _index = Feedrates.IndexOf(Feedrate);
        }
    }
}