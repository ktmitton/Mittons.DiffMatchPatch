using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Mittons.DiffMatchPatch.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class StringBuildingBenchmark
{
    public static string LongString = Enumerable.Range(0, 32000).Select(i => i.ToString()).Aggregate((current, next) => current + next);

    public static string BuildStringUsingInterpolation(string input)
    {
        return $"Hello, {input}!";
    }

    public static string BuildStringUsingStringBuilder(string input)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("Hello, ");
        stringBuilder.Append(input);
        stringBuilder.Append("!");
        return stringBuilder.ToString();
    }

    [Benchmark]
    public void BenchmarkInterpolation()
    {
        BuildStringUsingInterpolation(LongString);
    }

    [Benchmark]
    public void BenchmarkStringBuilder()
    {
        BuildStringUsingStringBuilder(LongString);
    }
}
