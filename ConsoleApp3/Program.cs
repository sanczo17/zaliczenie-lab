class Program
{
    public static readonly object lockObj = new();

    static void Main(string[] args)
    {
        Console.WriteLine("Wybierz funkcję:");
        Console.WriteLine("1. y = 2x + 2x²");
        Console.WriteLine("2. y = 2x² + 3");
        Console.WriteLine("3. y = 3x² + 2x - 3");
        Console.WriteLine("4. y = 5x² + 8x - 1");
        Console.WriteLine("5. y = x²");

        int choice = int.Parse(Console.ReadLine());

        Func<double, double> function = choice switch
        {
            1 => x => 2 * x + 2 * x * x,
            2 => x => 2 * x * x + 3,
            3 => x => 3 * x * x + 2 * x - 3,
            4 => x => 5 * x * x + 8 * x - 1,
            5 => x => Math.Pow(x,2),
            _ => throw new ArgumentException("Nieprawidłowy wybór funkcji.")
        };

        Console.Write("Podaj liczbę przedziałów: ");
        int intervals = int.Parse(Console.ReadLine());

        List<(double Start, double End)> ranges = new();

        for (int i = 0; i < intervals; i++)
        {
            Console.Write($"Podaj początek przedziału {i + 1}: ");
            double start = double.Parse(Console.ReadLine());
            Console.Write($"Podaj koniec przedziału {i + 1}: ");
            double end = double.Parse(Console.ReadLine());

            ranges.Add((start, end));
        }
        double a = ranges.First().Start;
        double b = ranges.Last().End;

        Console.WriteLine("Wybierz metodę przetwarzania:");
        Console.WriteLine("1. TPL (Parallel)");
        Console.WriteLine("2. ThreadPool");
        Console.WriteLine("3. Thread");
        Console.WriteLine("4. All");

        int methodChoice = int.Parse(Console.ReadLine());
        
        List<IProcessingMethod> processors = methodChoice switch
        {
            1 => new List<IProcessingMethod> { new TplProcessor() },
            2 => new List<IProcessingMethod> { new ThreadPoolProcessor() },
            3 => new List<IProcessingMethod> { new ThreadProcessor() },
            4 => new List<IProcessingMethod> { new TplProcessor(), new ThreadPoolProcessor(), new ThreadProcessor() },
            _ => throw new ArgumentException("Nieprawidłowy wybór metody.")
        };

        CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;

        foreach (var processor in processors)
        {
            Console.WriteLine($"\nUruchamianie: {processor.GetType().Name}");
            processor.Process(ranges, function, token);
        }

        Console.WriteLine("Rozpoczęto obliczenia...");
        Task.Run(() =>
        {
            Console.WriteLine("Wciśnij 'q' aby przerwać.");
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    cts.Cancel();
                    break;
                }
            }
        });
        
        double result = IntegralCalculator.CalculateIntegralParallel(function, a, b, 1000, token);
        Console.WriteLine($"Wynik całki: {result}");

        Console.Write("Podaj nazwę pliku dla wykresu (np. wykres.png): ");
        string fileName = Console.ReadLine();
        IntegralCalculator.GeneratePlot(function, a, b, fileName);
    }
}
