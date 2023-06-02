using SeaPortModelling;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("МОДЕЛИРОВАНИЕ РАБОТЫ ПОРТА\n\nТеоретически ожидается:\n1) танкеров 1-3 типов — 665\n2) танкеров 4 типа - 180\n3) штормов - 166\n4) ср. время обслуживания танкеров 1-3 типов - 24,7\n5) ср. время обслуживания танкеров 1-4 типов - 23,9\n");

        Console.WriteLine("МОДЕЛИРОВАНИЕ РАБОТЫ ПОРТА БЕЗ СПЕЦИАЛЬНЫХ ТАНКЕРОВ");
        Run(20, false);

        Console.WriteLine("\nМОДЕЛИРОВАНИЕ РАБОТЫ ПОРТА СО СПЕЦИАЛЬНЫМИ ТАНКЕРАМИ");
        Run(60, true);
    }

    private static void Run(int N, bool specialTankers)
    {
        float sum = 0;
        float[] QueueTimesArray = new float[N];

        for (int i = 0; i < N; i++)
        {
            QueueTimesArray[i] += GetQueueTime(8640, new TimeGenerator(13, 10), specialTankers);
            sum += QueueTimesArray[i];
        }

        Console.WriteLine($"\nN* = {Math.Ceiling(CalculateDispersion(QueueTimesArray, sum / (float)N, N) / (0.2f * 0.2f) * (1.960f * 1.960f))}; N = {N}");
        Console.WriteLine($"Среднее время ожидания: {Math.Round(sum / (float)N, 3)}");
    }

    private static float CalculateDispersion(float[] xi, float expectation, int N)
    {
        float sum = 0;

        for (int i = 0; i < N; ++i)
            sum += (float)Math.Pow(xi[i] - expectation, 2);

        return sum / (float)N;
    }

    private static float GetQueueTime(int hours, TimeGenerator arriveTime, bool specialTankers)
    {

        Airport port = new Airport(3, arriveTime, new TimeGenerator(240, 24),
            new float[] { 0.25f, 0.55f, 0.2f }, new BadLandingConditions(new TimeGenerator(4, 2), 1.0f / 48.0f));

        port.StartModelling(hours, specialTankers);

        Console.WriteLine("| ср. время ожидания: {0, 5:N3} | танкеров прибыло: {1}, из них 4 типа: {2} | штормов {3} | ср. время обслуживания {4, 4:N1} |",
            Math.Round(port.QueueTime, 3), port.TankersArrived, port.SpecialTankersArrived, port.StormAmount, Math.Round(port.GetAverageSpentTime(), 1));

        return port.QueueTime;
    }
}