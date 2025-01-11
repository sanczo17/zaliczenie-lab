interface IProcessingMethod
{
    void Process(List<(double Start, double End)> ranges, Func<double, double> function, CancellationToken token);
}