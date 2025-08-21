using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Mittons.DiffMatchPatch.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class BuildArrayBenchmarks
{
    public static void BuildWithFor(int size)
    {
        int[] v1 = new int[size];
        int[] v2 = new int[size];

        for (int x = 0; x < size; x++)
        {
            v1[x] = -1;
            v2[x] = -1;
        }
    }

    public static void BuildWithLinq(int size)
    {
        int[] v1 = [.. Enumerable.Repeat(-1, size)];
        int[] v2 = [.. Enumerable.Repeat(-1, size)];
    }

    [Benchmark]
    public void BenchmarkFor()
    {
        BuildWithFor(32000);
    }

    [Benchmark]
    public void BenchmarkLinq()
    {
        BuildWithLinq(32000);
    }
}
