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
            int step = Math.Max(1, n / progressWidth); // Zapobieganie dzieleniu przez zero

            for (int i = 1; i <= n; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Obliczenia przerwane.");
                    return 0;
                }

                double x = a + i * h;
                sum += func(x);

                if (i % step == 0)
                {
                    int progress = (int)((double)i / n * progressWidth);
                    Console.Write($"\r0%[{new string('=', progress)}{new string(' ', progressWidth - progress)}]100%");
                }
            }

            Console.WriteLine($"\r0%[{new string('=', progressWidth)}]100%");
        }

        return sum * h;
    }

    public static void GeneratePlot(Func<double, double> func, double a, double b, string fileName)
    {
        var plt = new ScottPlot.Plot();

        int points = 10000;
        double[] xs = new double[points];
        double[] ys = new double[points];
        double step = (b - a) / (points - 1);

        for (int i = 0; i < points; i++)
        {
            xs[i] = a + i * step;
            ys[i] = func(xs[i]);
        }

        plt.Add.Scatter(xs, ys);
        plt.Title($"Wykres funkcji na przedziale [{a}, {b}]");
        plt.XLabel("x");
        plt.YLabel("y");

        try
        {
            plt.SavePng(fileName, 1920, 1024);
            Console.WriteLine($"Wykres zapisano jako: {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd zapisu wykresu: {ex.Message}");
        }
    }

    public static double CalculateIntegralParallel(Func<double, double> func, double a, double b, int n, CancellationToken token)
    {
        double h = (b - a) / n;
        double sum = 0.0;

        object lockObj = new();

        Parallel.For(1, n + 1, (i, state) =>
        {
            if (token.IsCancellationRequested)
            {
                state.Stop();
                return;
            }

            double x = a + i * h;
            double fx = func(x);

            lock (lockObj)
            {
                sum += fx;
            }
        });

        sum += 0.5 * (func(a) + func(b));
        return sum * h;
    }
}
