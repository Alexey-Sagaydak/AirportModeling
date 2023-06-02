using AirportModelling;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("МОДЕЛИРОВАНИЕ РАБОТЫ АЭРОПОРТА (7 дней)\n\nТеоретически ожидается:\n1) самолетов — 1400\n2) Плохих взлетно-посадочных условий - 8\n4) ср. время обслуживания самолетов - 7.22 мин\n");

        Run(15, false);
    }

    private static void Run(int N, bool specialAirplanes)
    {
        float sum = 0;
        float[] QueueTimesArray = new float[N];

        for (int i = 0; i < N; i++)
        {
            QueueTimesArray[i] += GetQueueTime(10080, new TimeGenerator(7.22f, 4), specialAirplanes);
            sum += QueueTimesArray[i];
        }

        Console.WriteLine($"\nN* = {Math.Ceiling(CalculateDispersion(QueueTimesArray, sum / N, N) / (0.2f * 0.2f) * (1.960f * 1.960f))}; N = {N}");
        Console.WriteLine($"Среднее время ожидания: {Math.Round(sum / N, 3)}");
    }

    private static float CalculateDispersion(float[] xi, float expectation, int N)
    {
        float sum = 0;

        for (int i = 0; i < N; ++i)
            sum += (float)Math.Pow(xi[i] - expectation, 2);

        return sum / N;
    }

    private static float GetQueueTime(int hours, TimeGenerator arriveTime, bool specialAirplanes)
    {

        Airport port = new Airport(2, arriveTime, new TimeGenerator(240, 24),
            new float[] { 0.25f, 0.55f, 0.2f }, new BadLandingConditions(new TimeGenerator(150, 30), 1.0f / 2880.0f));

        port.StartModelling(hours, specialAirplanes);

        Console.WriteLine("| ср. время ожидания (мин): {0, 5:N3} | самолетов прибыло: {1} | плохих взл./пос. условий {2} | ср. время обслуживания (мин) {3, 4:N1} |",
            Math.Round(port.QueueTime, 3), port.AirplanesArrived, port.BadLandingConditionsAmount, Math.Round(port.GetAverageSpentTime(), 1));

        return port.QueueTime;
    }
}