using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Mittons.DiffMatchPatch.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class DeltaBenchmarks
{
    public static List<Diff> Diffs = [
        new Diff(Operation.EQUAL, "jump"),
        new Diff(Operation.DELETE, "s"),
        new Diff(Operation.INSERT, "ed"),
        new Diff(Operation.EQUAL, " over "),
        new Diff(Operation.DELETE, "the"),
        new Diff(Operation.INSERT, "a"),
        new Diff(Operation.EQUAL, " lazy"),
        new Diff(Operation.INSERT, "old dog"),
    ];

    public static string BuildFromAppend(IEnumerable<Diff> diffs)
    {
        StringBuilder text = new();

        foreach (Diff aDiff in diffs)
        {
            if (aDiff.Text.Length == 0)
            {
                continue;
            }

            switch (aDiff.Operation)
            {
                case Operation.INSERT:
                    text.Append($"+{aDiff.ToUriEncodedString()}\t");
                    break;
                case Operation.DELETE:
                    text.Append($"-{aDiff.Text.Length}\t");
                    break;
                case Operation.EQUAL:
                    text.Append($"={aDiff.Text.Length}\t");
                    break;
            }
        }

        return text.Length == 0 ? string.Empty : text.ToString()[..^1];
    }

    public static string BuildFromAppend2(IEnumerable<Diff> diffs)
    {
        StringBuilder text = new();

        foreach (Diff aDiff in diffs)
        {
            if (aDiff.Text.Length == 0)
            {
                continue;
            }

            text.Append(aDiff.ToDeltaEncodedString2());
        }

        return text.Length == 0 ? string.Empty : text.ToString()[..^1];
    }

    public static string BuildFromJoin(IEnumerable<Diff> diffs)
    {
        StringBuilder text = new();

        text.AppendJoin('\t', diffs.Select(diff => diff.Operation switch
        {
            Operation.INSERT => $"+{diff.ToUriEncodedString()}",
            Operation.DELETE => $"-{diff.Text.Length}",
            Operation.EQUAL => $"={diff.Text.Length}",
            _ => throw new ArgumentOutOfRangeException()
        }));

        return text.ToString();
    }

    public static string BuildFromJoin2(IEnumerable<Diff> diffs)
    {
        StringBuilder text = new();

        text.AppendJoin('\t', diffs.Select(diff => diff.ToDeltaEncodedString()));

        return text.ToString();
    }

    [Benchmark]
    public void BenchmarkAppend()
    {
        BuildFromAppend(Diffs);
    }

    [Benchmark]
    public void BenchmarkJoin()
    {
        BuildFromJoin(Diffs);
    }

    [Benchmark]
    public void BenchmarkAppend2()
    {
        BuildFromAppend2(Diffs);
    }

    [Benchmark]
    public void BenchmarkJoin2()
    {
        BuildFromJoin2(Diffs);
    }
}
