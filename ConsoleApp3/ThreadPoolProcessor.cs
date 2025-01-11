using System.Diagnostics;

class ThreadPoolProcessor : IProcessingMethod
{
    public void Process(List<(double Start, double End)> ranges, Func<double, double> function, CancellationToken token)
    {
        CountdownEvent countdown = new CountdownEvent(ranges.Count);

        foreach (var range in ranges)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                double result = IntegralCalculator.CalculateIntegral(function, range.Start, range.End, 1000, token, range);
                sw.Stop();

                lock (Program.lockObj)
                {
                    Console.WriteLine($"Przedział: [{range.Start}, {range.End}] Wynik: {result} Czas: {sw.ElapsedMilliseconds} ms");
                }

                countdown.Signal();
            });
        }

        countdown.Wait();
    }
}