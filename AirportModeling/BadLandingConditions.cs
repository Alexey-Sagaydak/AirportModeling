namespace AirportModelling;

public class BadLandingConditions
{
    public TimeGenerator Duration { get; }
    
    public float CurrentDuration { get; set;  }
    public float ArrivalTime { get; set;  }
    private float lambda { get; }
    
    public BadLandingConditions(TimeGenerator duration, float _lambda)
    {
        if (duration == null)
            throw new ArgumentNullException(nameof(duration));
        Duration = duration;
        lambda = _lambda;
        Get();
    }

    public void Get()
    {
        Random r = new Random();
        ArrivalTime += CurrentDuration; 
        ArrivalTime += 1 / lambda * (float) -Math.Log(1 - r.NextDouble());
        GenerateDuration();
    }

    public void GenerateDuration()
    {
        CurrentDuration = Duration.Get();
    }
}