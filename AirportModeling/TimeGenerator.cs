namespace SeaPortModelling;

public class TimeGenerator
{
    public float Base { get; }
    public float Interval { get; }

    public TimeGenerator(float _base, float _interval)
    {
        Base = _base;
        Interval = _interval;
    }

    public float Get()
    {
        Random r = new Random();
        float max = Base + Interval, min = Base - Interval;
        return (float) r.NextDouble() * (max - min) + min;
    }
}