using System.Diagnostics;


class TplProcessor : IProcessingMethod
{
    public void Process(List<(double Start, double End)> ranges, Func<double, double> function, CancellationToken token)
    {
        Parallel.ForEach(ranges, range =>
        {
            Stopwatch sw = Stopwatch.StartNew();
            double result = IntegralCalculator.CalculateIntegral(function, range.Start, range.End, 1000, token, range);
            sw.Stop();

            lock (Program.lockObj)
            {
                Console.WriteLine($"Przedział: [{range.Start}, {range.End}] Wynik: {result} Czas: {sw.ElapsedMilliseconds} ms");
            }
        });

    }
}