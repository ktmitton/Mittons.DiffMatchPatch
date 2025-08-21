using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Mittons.DiffMatchPatch.Benchmark.Original;
using Mittons.DiffMatchPatch.Extensions;

namespace Mittons.DiffMatchPatch.Benchmark.CommonMatches;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class CommonOverlapBenchmarks
{
    public const int OperationsPerInvoke = 1;

    public static readonly (string left, string right)[] TestCases = [
        ("abc", "xyz"),
        ("1234abcdef", "1234xyz"),
        ("1234", "1234xyz"),
        ("hello world", "hello universe"),
        ("abcdefg", "abcxyz"),
        ("12341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234abcdef", "12341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234xyz"),

        ("abc", "xyz"),
        ("abcdef1234", "xyz1234"),
        ("1234", "xyz1234"),
        ("world hello", "universe hello"),
        ("defgabc", "xyzabc"),
        ("abcdef12341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234", "xyz12341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234"),

        ("1234567890", "abcdef"),
        ("12345", "23"),
        ("1234567890", "a345678z"),
        ("a345678z", "1234567890"),
        ("abc56789z", "1234567890"),
        ("a23456xyz", "1234567890"),
        ("121231234123451234123121", "a1234123451234z"),
        ("x-=-=-=-=-=-=-=-=-=-=-=-=", "xx-=-=-=-=-=-=-="),
        ("-=-=-=-=-=-=-=-=-=-=-=-=y", "-=-=-=-=-=-=-=yy"),
        ("qHilloHelloHew", "xHelloHeHulloy"),
        ("abc12341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234def", "def12341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234123412341234abc"),

        ("", "abcd"),
        ("123456", "abcd"),
        ("abc", "abcd"),
        ("123456xxx", "xxxabcd"),
        ("fi", "\ufb01i"),

        ("abcd", ""),
        ("abcd", "123456"),
        ("abcd", "abc"),
        ("xxxabcd", "123456xxx"),
        ("\ufb01i", "fi"),
    ];

    public static diff_match_patch OriginalDiffMatchPatch = new();

    public static List<(bool hasCommonOverlap, int commonOverlapLength, string? commonOverlap)> LegacyApproach()
    {
        List<(bool hasCommonOverlap, int commonOverlapLength, string? commonOverlap)> results = [];
        foreach (var (left, right) in TestCases)
        {
            var commonOverlapLength = OriginalDiffMatchPatch.diff_commonOverlap(left, right);
            var hasCommonOverlap = commonOverlapLength > 0;
            results.Add((hasCommonOverlap, commonOverlapLength, hasCommonOverlap ? left.Substring(left.Length - commonOverlapLength) : null));
        }
        return results;
    }

    public static List<(bool hasCommonOverlap, int commonOverlapLength)> NewApproach()
    {
        List<(bool hasCommonOverlap, int commonOverlapLength)> results = [];
        foreach (var (left, right) in TestCases)
        {
            var hasCommonOverlap = left.TryFindCommonOverlap(
                right,
                out var commonOverlapSpan
            );

            if (hasCommonOverlap)
            {
                var commonOverlapLength = commonOverlapSpan.Length;
                results.Add((hasCommonOverlap, commonOverlapLength));
            }
            else
            {
                results.Add((hasCommonOverlap, 0));
            }
        }
        return results;
    }

    public static List<(bool hasCommonOverlap, int commonOverlapLength, bool hasCommonOverlap2, int commonOverlapLength2, string[]? vals)> LegacyDualApproach()
    {
        List<(bool hasCommonOverlap, int commonOverlapLength, bool hasCommonOverlap2, int commonOverlapLength2, string[]? vals)> results = [];
        foreach (var (left, right) in TestCases)
        {
            var commonOverlapLength = OriginalDiffMatchPatch.diff_commonOverlap(left, right);
            var hasCommonOverlap = commonOverlapLength > 0;

            var commonOverlapLength2 = OriginalDiffMatchPatch.diff_commonOverlap(right, left);
            var hasCommonOverlap2 = commonOverlapLength2 > 0;

            if (hasCommonOverlap || hasCommonOverlap2)
            {
                string leftPrefix = string.Empty;
                string leftSuffix = string.Empty;
                string rightPrefix = string.Empty;
                string rightSuffix = string.Empty;
                string commonOverlap = string.Empty;

                if (commonOverlapLength >= commonOverlapLength2)
                {
                    leftPrefix = left[..(left.Length - commonOverlapLength)];
                    rightSuffix = right[commonOverlapLength..];
                    commonOverlap = left[(left.Length - commonOverlapLength2)..];
                }
                else
                {
                    leftSuffix = left[commonOverlapLength2..];
                    rightPrefix = right[..(right.Length - commonOverlapLength2)];
                    commonOverlap = right[(right.Length - commonOverlapLength2)..];
                }
                results.Add((hasCommonOverlap, commonOverlapLength, hasCommonOverlap2, commonOverlapLength2, new[] { leftPrefix, leftSuffix, rightPrefix, rightSuffix, commonOverlap }));
            }
            else
            {
                results.Add((hasCommonOverlap, 0, hasCommonOverlap2, 0, null));
            }
        }
        return results;
    }

    public static List<bool> NewDualApproach()
    {
        List<bool> results = [];
        foreach (var (left, right) in TestCases)
        {
            var hasCommonOverlap = left.TryFindCommonOverlap(
                right,
                out var leftPrefixSpan,
                out var leftSuffixSpan,
                out var rightPrefixSpan,
                out var rightSuffixSpan,
                out var commonOverlapSpan
            );
            results.Add(hasCommonOverlap);
        }

        return results;
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void BenchmarkLegacy()
    {
        LegacyApproach();
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void BenchmarkNew()
    {
        NewApproach();
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void BenchmarkLegacyDual()
    {
        LegacyDualApproach();
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void BenchmarkNewDual()
    {
        NewDualApproach();
    }
}
