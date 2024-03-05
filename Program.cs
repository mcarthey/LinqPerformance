using BenchmarkDotNet.Running;

namespace LinqPerformance;

public class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Benchmark>();
    }
}