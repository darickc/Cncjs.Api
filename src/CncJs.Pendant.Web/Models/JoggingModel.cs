namespace CncJs.Pendant.Web.Models;

public class JoggingModel
{
    public double Distance { get; set; } = 1;
    public List<double> Distances => new ()
    {
        .001,.002,.003,.005,.01,.02,.03,.05,.1,.2,.3,.5,1,2,3,5,10,20,30,50,100,200,300,500
    };

    public bool CanIncrease => _index < Distances.Count;
    public bool CanDecrease => _index > 0;

    private int _index;

    public JoggingModel()
    {
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
}