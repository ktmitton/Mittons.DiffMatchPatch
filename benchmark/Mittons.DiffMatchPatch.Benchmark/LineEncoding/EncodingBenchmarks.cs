using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Mittons.DiffMatchPatch.Benchmark.Original;
using Mittons.DiffMatchPatch.Extensions;
using Mittons.DiffMatchPatch.Models;

namespace Mittons.DiffMatchPatch.Benchmark.LineEncoding;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class EncodingBenchmarks
{
    public static readonly (string left, string right)[] TestCases = [
        ("alpha\nbeta\nalpha\n", "beta\nalpha\nbeta\n"),
        ("", "alpha\r\nbeta\r\n\r\n\r\n"),
        ("a", "b"),
        ("", string.Join("", Enumerable.Range(0, 300).Select(x => $"{x}\n"))),
        ("", string.Join("", Enumerable.Range(0, 300).Select(x => $"jfkldjaskl;fjdskl;jflk;dsjlk;fdjs;akljfdl;ksajfkl;djal;kfjdl;skjflk;dsjal;kfjdsl;kajflkd;sjalk;fdjlak;jfdlkjalkfdjslak;jfdkl;sajflkdjsakl;fjds;klajfl;kdjakl;fjdkl;ajfldk;sajflk;djalk;fjdkl;ajflk;dsjal;kfdjsaklfjdklsajflk;dsjakl;fdjsal;kfjdskl;ajflk;dsjal;fkdjsaklfjdkl;ajfkl;dsajl;kfdjsal;kfjdkl;ajfkldjafkl;djalk;fjdlk;ajfdkl;jaflk;dsjakl;fsd{x}\n"))),
    ];

    public static diff_match_patch OriginalDiffMatchPatch = new();

    public static List<object[]> LegacyApproach()
    {
        List<object[]> results = [];
        foreach (var (left, right) in TestCases)
        {
            results.Add(OriginalDiffMatchPatch.diff_linesToChars(left, right));
        }
        return results;
    }

    public static List<HashedTexts> NewApproach()
    {
        List<HashedTexts> results = [];
        foreach (var (left, right) in TestCases)
        {
            results.Add(left.CompressTexts(right));
        }
        return results;
    }

    [Benchmark]
    public void BenchmarkLegacy()
    {
        LegacyApproach();
    }

    [Benchmark]
    public void BenchmarkNew()
    {
        NewApproach();
    }
}
