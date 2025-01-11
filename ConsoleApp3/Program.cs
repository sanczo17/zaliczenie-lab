class Program
{
    public static readonly object lockObj = new();

    static void Main(string[] args)
    {
        Console.WriteLine("Wybierz funkcję:");
        Console.WriteLine("1. y = 2x + 2x²");
        Console.WriteLine("2. y = 2x² + 3");
        Console.WriteLine("3. y = 3x² + 2x - 3");

        int choice = int.Parse(Console.ReadLine());

        Func<double, double> function = choice switch
        {
            1 => x => 2 * x + 2 * x * x,
            2 => x => 2 * x * x + 3,
            3 => x => 3 * x * x + 2 * x - 3,
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

        Console.WriteLine("Wybierz metodę przetwarzania:");
        Console.WriteLine("1. TPL (Parallel)");
        Console.WriteLine("2. ThreadPool");
        Console.WriteLine("3. Thread");

        int methodChoice = int.Parse(Console.ReadLine());
        IProcessingMethod processor = methodChoice switch
        {
            1 => new TplProcessor(),
            2 => new ThreadPoolProcessor(),
            3 => new ThreadProcessor(),
            _ => throw new ArgumentException("Nieprawidłowy wybór metody.")
        };

        CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;

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

        processor.Process(ranges, function, token);
    }
}
