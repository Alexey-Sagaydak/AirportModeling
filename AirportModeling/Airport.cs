namespace AirportModelling;

public class Airport
{
    public Runway[] Runways { get; set; }
    private float[] AirplaneFrequencies { get; }
    private TimeGenerator SpecialAirplaneCycle { get; }
    private TimeGenerator ArriveTimeGenerator { get; }
    private BadLandingConditions BadLandingConditions { get; }
    public int SpecialAirplanesArrived { get; set; }
    public int BadLandingConditionsAmount { get; set; }
    private Airplane[] SpecialAirplanes { get; }
    private int Counter { get; set; }
    public float QueueTime { get; set; }
    public int AirplanesArrived { get; set;  }
    private float ModellingDuration;

    public Airport(int runwaysAmount, TimeGenerator arriveTimeGenerator, TimeGenerator specialTankerCycle,
        float[] airplaneFrequencies, BadLandingConditions storm)
    {
        Runways = new Runway[runwaysAmount];
        SpecialAirplanes = new Airplane[5];
        InitializeRunways();
        if (arriveTimeGenerator == null)
            throw new ArgumentNullException(nameof(arriveTimeGenerator));
        if (specialTankerCycle == null)
            throw new ArgumentNullException(nameof(specialTankerCycle));
        if (storm == null)
            throw new ArgumentNullException(nameof(storm));
        CheckTankerFrequencies(airplaneFrequencies);

        BadLandingConditions = storm;
        ArriveTimeGenerator = arriveTimeGenerator;
        SpecialAirplaneCycle = specialTankerCycle;
        AirplaneFrequencies = airplaneFrequencies;
    }

    private void InitializeSpecialAirplanes(float currentTime)
    {
        for (int i = 0; i < SpecialAirplanes.Length; i++)
            SpecialAirplanes[i] = new Airplane(new TimeGenerator(21, 3), 0, currentTime + SpecialAirplaneCycle.Get());
    }

    private void InitializeRunways()
    {
        for (int i = 0; i < Runways.Length; i++)
            Runways[i] = new Runway();
    }

    private void CheckTankerFrequencies(float[] frequencies)
    {
        foreach (float frequency in frequencies)
            if (frequency < 0 || frequency > 1)
                throw new ArgumentException(nameof(frequency));
    }

    public void StartModelling(float modellingDuration, bool specialAirplanes)
    {
        ModellingDuration = modellingDuration;
        float currentTime = 0;

        if (specialAirplanes)
            InitializeSpecialAirplanes(currentTime);
        
        while (currentTime < modellingDuration)
        {
            if (currentTime >= BadLandingConditions.ArrivalTime && currentTime <= BadLandingConditions.ArrivalTime + BadLandingConditions.CurrentDuration)
            {
                currentTime = BadLandingConditions.ArrivalTime + BadLandingConditions.CurrentDuration;
                Counter++;
            }
            else
            {
                Airplane airplane = GenerateCommonTanker(currentTime);
                GetRunway(airplane);
                if (specialAirplanes)
                    CheckSpecialAirplanesArrival(currentTime);
                currentTime = airplane.ArriveTime;
                QueueTime += airplane.QueueTime;
            }

            if (BadLandingConditions.ArrivalTime <= currentTime)
            {
                BadLandingConditions.Get();
                BadLandingConditionsAmount++;
            }
        }

        CalculateAirplanesArrived();
        QueueTime /= AirplanesArrived;
        CalculateSpentTime(modellingDuration);
    }

    private void CalculateAirplanesArrived()
    {
        foreach (Runway runway in Runways)
            AirplanesArrived += runway.ServedAmount;
    }
    private void CalculateSpentTime(float modellingDuration)
    {
        foreach (Runway runway in Runways)
            runway.SpentTime = runway.ReleaseTime - runway.IdleTime;
    }

    public float GetAverageSpentTime()
    {
        float sum = 0;

        foreach (Runway runway in Runways)
        {
            sum += (runway.SpentTime == ModellingDuration) ? 0 : runway.SpentTime;
        }
            

        return sum / AirplanesArrived;
    }

    private void CheckSpecialAirplanesArrival(float currentTime)
    {
        foreach (Airplane airplane in SpecialAirplanes)
        {
            if (currentTime > airplane.ArriveTime)
            {
                GetRunway(airplane);
                SpecialAirplanesArrived++;
                airplane.ArriveTime = currentTime + SpecialAirplaneCycle.Get();
            }
        }
    }

    private int GetAirplanesAmount()
    {
        int count = 0;
        foreach (Runway runway in Runways)
            count += runway.ServedAmount;
        return count;
    }

    private void GetRunway(Airplane airplane)
    {
        foreach (Runway runway in Runways)
        {
            if (airplane.ArriveTime >= runway.ReleaseTime)
            {
                runway.IdleTime += airplane.ArriveTime - runway.ReleaseTime;
                runway.ReleaseTime = airplane.ArriveTime + airplane.LoadTime.Get();
                runway.ServedAmount++;
                return;
            }
        }

        Random r = new Random();
        int value = r.Next(0, Runways.Length);
        airplane.QueueTime += Runways[value].ReleaseTime - airplane.ArriveTime;
        Runways[value].ReleaseTime += airplane.LoadTime.Get();
        Runways[value].ServedAmount++;
    }

    private Airplane GenerateCommonTanker(float currentTime)
    {
        Random r = new Random();
        float arriveTime = currentTime + ArriveTimeGenerator.Get(), airplaneType = (float)r.NextDouble();
        TimeGenerator timeGenerator;

        //if (airplaneType < AirplaneFrequencies[0])
        //    timeGenerator = new TimeGenerator(18, 2);
        //else if (airplaneType < AirplaneFrequencies[0] + AirplaneFrequencies[1])
        //    timeGenerator = new TimeGenerator(24, 3);
        //else
        timeGenerator = new TimeGenerator(12, 5);

        return new Airplane(timeGenerator, 0, arriveTime);
    }
}