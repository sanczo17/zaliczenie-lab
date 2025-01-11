using System.Diagnostics;

class ThreadProcessor : IProcessingMethod
{
    public void Process(List<(double Start, double End)> ranges, Func<double, double> function, CancellationToken token)
    {
        List<Thread> threads = new();

        foreach (var range in ranges)
        {
            var thread = new Thread(() =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                double result = IntegralCalculator.CalculateIntegral(function, range.Start, range.End, 1000, token, range);
                sw.Stop();

                lock (Program.lockObj)
                {
                    Console.WriteLine($"Przedział: [{range.Start}, {range.End}] Wynik: {result} Czas: {sw.ElapsedMilliseconds} ms");
                }
            });
             
            threads.Add(thread);
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}