class IntegralCalculator
{
    public static double CalculateIntegral(Func<double, double> func, double a, double b, int n, CancellationToken token, (double Start, double End) range)
    {
        double h = (b - a) / n;
        double sum = 0.5 * (func(a) + func(b));
        int progressWidth = 20;

        lock (Program.lockObj)
        {
            Console.WriteLine($"Przedział: [{range.Start}, {range.End}]");
            for (int i = 1; i <= n; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Obliczenia przerwane.");
                    return 0;
                }

                double x = a + i * h;
                sum += func(x);

                if (i % (n / progressWidth) == 0)
                {
                    int progress = (int)((double)i / n * progressWidth);
                    Console.Write($"\r0%[{new string('=', progress)}{new string(' ', progressWidth - progress)}]100%");
                    //Thread.Sleep(100);
                }
            }

            Console.WriteLine($"\r0%[{new string('=', progressWidth)}]100%");
        }

        return sum * h;
    }
}