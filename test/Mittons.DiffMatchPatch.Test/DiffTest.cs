using System.Text;
using Mittons.DiffMatchPatch.Extensions;
using Mittons.DiffMatchPatch.Models;
using Mittons.DiffMatchPatch.Types;

namespace Mittons.DiffMatchPatch.Test;

public class DiffTest
{
    [Test]
    [Arguments("abc", "xyz", 0)]
    [Arguments("1234abcdef", "1234xyz", 4)]
    [Arguments("1234", "1234xyz", 4)]
    public async Task CommonPrefixLength(string left, string right, int expectedSharedCharacterCount)
    {
        var actualSharedCharacterCount = left.CommonPrefixLength(right);

        await Assert.That(actualSharedCharacterCount).IsEqualTo(expectedSharedCharacterCount);
    }

    [Test]
    [Arguments("", "abcd", 0)]
    [Arguments("123456", "abcd", 0)]
    public async Task CommonOverlapLength_WhenThereIsNoOverlap_ExpectZero(string left, string right, int expectedSharedCharacterCount)
    {
        var actualSharedCharacterCount = left.CommonOverlapLength(right);

        await Assert.That(actualSharedCharacterCount).IsEqualTo(expectedSharedCharacterCount);
    }

    [Test]
    [Arguments("abc", "abcd", 3)]
    [Arguments("123456xxx", "xxxabcd", 3)]
    public async Task CommonOverlapLength_WhenThereIsOverlap_ExpectCharacterCount(string left, string right, int expectedSharedCharacterCount)
    {
        var actualSharedCharacterCount = left.CommonOverlapLength(right);

        await Assert.That(actualSharedCharacterCount).IsEqualTo(expectedSharedCharacterCount);
    }

    [Test]
    [Arguments("fi", "\ufb01i", 0)]
    public async Task CommonOverlapLength_WhenOverlapMixesUnicodeLigaturesWithAscii_ExpectZero(string left, string right, int expectedSharedCharacterCount)
    {
        var actualSharedCharacterCount = left.CommonOverlapLength(right);

        await Assert.That(actualSharedCharacterCount).IsEqualTo(expectedSharedCharacterCount);
    }

    [Test]
    [Arguments("abc", "xyz", 0)]
    [Arguments("abcdef1234", "xyz1234", 4)]
    [Arguments("1234", "xyz1234", 4)]
    public async Task CommonSuffixLength(string left, string right, int expectedSharedCharacterCount)
    {
        var actualSharedCharacterCount = left.CommonSuffixLength(right);

        await Assert.That(actualSharedCharacterCount).IsEqualTo(expectedSharedCharacterCount);
    }

    [Test]
    [Arguments("1234567890", "abcdef")]
    [Arguments("12345", "23")]
    public async Task CommonMiddle_WhenThereIsNoCommonMiddle_ExpectEmptyDetails(string left, string right)
    {
        var expectedSharedCharacterCount = 0;

        using var cancellationTokenSource = new CancellationTokenSource();
        CommonMiddleDetails actualDetails = left.CommonMiddle(right, cancellationTokenSource.Token);

        await Assert.That(actualDetails.CommonMiddle.Length).IsEqualTo(expectedSharedCharacterCount);
    }

    [Test]
    [Arguments("1234567890", "a345678z", "12", "90", "a", "z", "345678")]
    [Arguments("a345678z", "1234567890", "a", "z", "12", "90", "345678")]
    [Arguments("abc56789z", "1234567890", "abc", "z", "1234", "0", "56789")]
    [Arguments("a23456xyz", "1234567890", "a", "xyz", "1", "7890", "23456")]
    [Arguments("121231234123451234123121", "a1234123451234z", "12123", "123121", "a", "z", "1234123451234")]
    [Arguments("x-=-=-=-=-=-=-=-=-=-=-=-=", "xx-=-=-=-=-=-=-=", "", "-=-=-=-=-=", "x", "", "x-=-=-=-=-=-=-=")]
    [Arguments("-=-=-=-=-=-=-=-=-=-=-=-=y", "-=-=-=-=-=-=-=yy", "-=-=-=-=-=", "", "", "y", "-=-=-=-=-=-=-=y")]
    [Arguments("qHilloHelloHew", "xHelloHeHulloy", "qHillo", "w", "x", "Hulloy", "HelloHe")]
    public async Task CommonMiddle_WhenThereIsACommonMiddle_ExpectDetails(string left, string right,
        string expectedLeftPrefix,
        string expectedLeftSuffix,
        string expectedRightPrefix,
        string expectedRightSuffix,
        string expectedCommonMiddle)
    {
        CommonMiddleDetails expectedMiddle = new(expectedLeftPrefix, expectedLeftSuffix, expectedRightPrefix, expectedRightSuffix, expectedCommonMiddle);
        using var cancellationTokenSource = new CancellationTokenSource();

        CommonMiddleDetails actualDetails = left.CommonMiddle(right, cancellationTokenSource.Token);

        var actualLeftPrefix = actualDetails.LeftPrefix.ToString();
        var actualLeftSuffix = actualDetails.LeftSuffix.ToString();
        var actualRightPrefix = actualDetails.RightPrefix.ToString();
        var actualRightSuffix = actualDetails.RightSuffix.ToString();
        var actualCommonMiddle = actualDetails.CommonMiddle.ToString();

        await Assert.That(actualLeftPrefix).IsEqualTo(expectedLeftPrefix);
        await Assert.That(actualLeftSuffix).IsEqualTo(expectedLeftSuffix);
        await Assert.That(actualRightPrefix).IsEqualTo(expectedRightPrefix);
        await Assert.That(actualRightSuffix).IsEqualTo(expectedRightSuffix);
        await Assert.That(actualCommonMiddle).IsEqualTo(expectedCommonMiddle);
    }

    [Test]
    [Arguments("qHilloHelloHew", "xHelloHeHulloy")]
    public async Task CommonMiddle_WhenThereNoCancellationToken_ExpectNoDetails(string left, string right)
    {
        var expectedSharedCharacterCount = 0;

        CommonMiddleDetails actualDetails = left.CommonMiddle(right, default);

        await Assert.That(actualDetails.CommonMiddle.Length).IsEqualTo(expectedSharedCharacterCount);
    }

    public static IEnumerable<(string leftText, string rightText, HashedTexts? hashedTexts)> HashTextsDatasource()
    {
        yield return (
            "alpha\nbeta\nalpha\n",
            "beta\nalpha\nbeta\n",
            new HashedTexts(
                ["alpha\n", "beta\n"],
                "\0\u0001\0",
                "\u0001\0\u0001"
            )
        );

        yield return (
            "",
            "alpha\r\nbeta\r\n\r\n\r\n",
            new HashedTexts(
                ["alpha\r\n", "beta\r\n", "\r\n"],
                "",
                "\0\u0001\u0002\u0002"
            )
        );

        yield return (
            "a",
            "b",
            new HashedTexts(
                ["a", "b"],
                "\0",
                "\u0001"
            )
        );

        yield return (
            "",
            string.Join("", Enumerable.Range(0, 300).Select(x => $"{x}\n")),
            new HashedTexts(
                [.. Enumerable.Range(0, 300).Select(x => $"{x}\n")],
                "",
                string.Join("", Enumerable.Range(0, 300).Select(x => $"{(char)x}"))
            )
        );
    }

    [Test]
    [MethodDataSource(nameof(HashTextsDatasource))]
    public async Task LinesToCharsTest(string leftText, string rightText, HashedTexts expectedHashedTexts)
    {
        var actualHashedTexts = leftText.CompressTexts(rightText);

        var a = actualHashedTexts.HashLookup;
        var b = expectedHashedTexts.HashLookup;
        await Assert.That(b).IsEquivalentTo(a);
        await Assert.That(actualHashedTexts).IsEquivalentTo(expectedHashedTexts);
    }

    public static IEnumerable<(IList<string> lookup, IEnumerable<Diff> encryptedDiff, IEnumerable<Diff> expectedResult)> DecompressDiffDatasource()
    {
        yield return (
            [
                "alpha\n",
                "beta\n",
            ],
            [
                new Diff(Operation.EQUAL, "\0\u0001\0"),
                new Diff(Operation.INSERT, "\u0001\0\u0001"),
            ],
            [
                new Diff(Operation.EQUAL, "alpha\nbeta\nalpha\n"),
                new Diff(Operation.INSERT, "beta\nalpha\nbeta\n"),
            ]
        );

        yield return (
            Enumerable.Range(0, 300).Select(x => $"{x}\n").ToList(),
            [
                new Diff(Operation.DELETE, string.Join("", Enumerable.Range(0, 300).Select(x => (char)x)))
            ],
            [
                new Diff(Operation.DELETE, string.Join("", Enumerable.Range(0, 300).Select(x => $"{x}\n")))
            ]
        );

        var originalText = string.Join("", Enumerable.Range(0, 66000).Select(x => $"{x}\n"));
        List<string> decompressionLookup = [];
        Dictionary<string, int> compressionLookup = [];
        var compressedLines = Extensions.StringExtensions.CompressLines(originalText, decompressionLookup, compressionLookup, 4);
        yield return (
            decompressionLookup,
            [
                new Diff(Operation.INSERT, string.Join("", compressedLines))
            ],
            [
                new Diff(Operation.INSERT, originalText)
            ]
        );
    }

    [Test]
    [MethodDataSource(nameof(DecompressDiffDatasource))]
    public async Task DecompressDiffTest(IList<string> lookup, IEnumerable<Diff> encryptedDiff, IEnumerable<Diff> expectedResult)
    {
        var actualResult = encryptedDiff.DecompressDiff(lookup);

        await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    }

    public static IEnumerable<(IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)> CleanupMergeDatasource()
    {
        yield return (
            [
                new Diff(Operation.EQUAL, "a"),
                new Diff(Operation.DELETE, "b"),
                new Diff(Operation.INSERT, "c"),
            ],
            [
                new Diff(Operation.EQUAL, "a"),
                new Diff(Operation.DELETE, "b"),
                new Diff(Operation.INSERT, "c"),
            ]
        );

        foreach (var operation in new Operation[] { Operation.EQUAL, Operation.DELETE, Operation.INSERT })
        {
            yield return (
                [
                    new Diff(operation, "a"),
                    new Diff(operation, "b"),
                    new Diff(operation, "c"),
                ],
                [
                    new Diff(operation, "abc"),
                ]
            );
        }

        yield return (
            [
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.INSERT, "b"),
                new Diff(Operation.DELETE, "c"),
                new Diff(Operation.INSERT, "d"),
                new Diff(Operation.EQUAL, "e"),
                new Diff(Operation.EQUAL, "f"),
            ],
            [
                new Diff(Operation.DELETE, "ac"),
                new Diff(Operation.INSERT, "bd"),
                new Diff(Operation.EQUAL, "ef"),
            ]
        );

        yield return (
            [
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.INSERT, "abc"),
                new Diff(Operation.DELETE, "dc"),
            ],
            [
                new Diff(Operation.EQUAL, "a"),
                new Diff(Operation.DELETE, "d"),
                new Diff(Operation.INSERT, "b"),
                new Diff(Operation.EQUAL, "c"),
            ]
        );

        yield return (
            [
                new Diff(Operation.EQUAL, "x"),
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.INSERT, "abc"),
                new Diff(Operation.DELETE, "dc"),
                new Diff(Operation.EQUAL, "y"),
            ],
            [
                new Diff(Operation.EQUAL, "xa"),
                new Diff(Operation.DELETE, "d"),
                new Diff(Operation.INSERT, "b"),
                new Diff(Operation.EQUAL, "cy"),
            ]
        );

        yield return (
            [
                new Diff(Operation.EQUAL, "a"),
                new Diff(Operation.INSERT, "ba"),
                new Diff(Operation.EQUAL, "c"),
            ],
            [
                new Diff(Operation.INSERT, "ab"),
                new Diff(Operation.EQUAL, "ac"),
            ]
        );

        yield return (
            [
                new Diff(Operation.EQUAL, "c"),
                new Diff(Operation.INSERT, "ab"),
                new Diff(Operation.EQUAL, "a"),
            ],
            [
                new Diff(Operation.EQUAL, "ca"),
                new Diff(Operation.INSERT, "ba"),
            ]
        );

        yield return (
            [
                new Diff(Operation.DELETE, "b"),
                new Diff(Operation.INSERT, "ab"),
                new Diff(Operation.EQUAL, "c"),
            ],
            [
                new Diff(Operation.INSERT, "a"),
                new Diff(Operation.EQUAL, "bc"),
            ]
        );

        yield return (
            [
                new Diff(Operation.EQUAL, ""),
                new Diff(Operation.INSERT, "a"),
                new Diff(Operation.EQUAL, "b"),
            ],
            [
                new Diff(Operation.INSERT, "a"),
                new Diff(Operation.EQUAL, "b"),
            ]
        );
    }

    public static IEnumerable<(IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)> CleanupMergeMultipassDatasource()
    {
        yield return (
            [
                new Diff(Operation.EQUAL, "a"),
                new Diff(Operation.DELETE, "b"),
                new Diff(Operation.EQUAL, "c"),
                new Diff(Operation.DELETE, "ac"),
                new Diff(Operation.EQUAL, "x")
            ],
            [
                new Diff(Operation.DELETE, "abc"),
                new Diff(Operation.EQUAL, "acx"),
            ]
        );

        yield return (
            [
                new Diff(Operation.EQUAL, "x"),
                new Diff(Operation.DELETE, "ca"),
                new Diff(Operation.EQUAL, "c"),
                new Diff(Operation.DELETE, "b"),
                new Diff(Operation.EQUAL, "a"),
            ],
            [
                new Diff(Operation.EQUAL, "xca"),
                new Diff(Operation.DELETE, "cba"),
            ]
        );
    }

    [Test]
    [MethodDataSource(nameof(CleanupMergeDatasource))]
    public async Task CleanupMergeTest(IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)
    {
        var actualResult = originalDiff.CleanupMerge();

        await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    }

    [Test]
    [Skip("We currently aren't doing multipass on this since that's only needed if this is called outside the normal workflow. Within the normal workflow, things _should_ be optimized enough that we don't have multiple groups that need checked together.")]
    [MethodDataSource(nameof(CleanupMergeMultipassDatasource))]
    public async Task CleanupMergeTest_Multipass(IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)
    {
        var actualResult = originalDiff.CleanupMerge();

        await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    }

    public static IEnumerable<(string description, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)> CleanupSemanticLosslessDatasource()
    {
        yield return ("Null case.", [], []);

        yield return (
            "Blank lines.",
            [
                new Diff(Operation.EQUAL, "AAA\r\n\r\nBBB"),
                new Diff(Operation.INSERT, "\r\nDDD\r\n\r\nBBB"),
                new Diff(Operation.EQUAL, "\r\nEEE"),
            ],
            [
                new Diff(Operation.EQUAL, "AAA\r\n\r\n"),
                new Diff(Operation.INSERT, "BBB\r\nDDD\r\n\r\n"),
                new Diff(Operation.EQUAL, "BBB\r\nEEE"),
            ]
        );

        yield return (
            "Line boundaries.",
            [
                new Diff(Operation.EQUAL, "AAA\r\nBBB"),
                new Diff(Operation.INSERT, " DDD\r\nBBB"),
                new Diff(Operation.EQUAL, " EEE"),
            ],
            [
                new Diff(Operation.EQUAL, "AAA\r\n"),
                new Diff(Operation.INSERT, "BBB DDD\r\n"),
                new Diff(Operation.EQUAL, "BBB EEE"),
            ]
        );

        yield return (
            "Word boundaries.",
            [
                new Diff(Operation.EQUAL, "The c"),
                new Diff(Operation.INSERT, "ow and the c"),
                new Diff(Operation.EQUAL, "at."),
            ],
            [
                new Diff(Operation.EQUAL, "The "),
                new Diff(Operation.INSERT, "cow and the "),
                new Diff(Operation.EQUAL, "cat."),
            ]
        );

        yield return (
            "Alphanumeric boundaries.",
            [
                new Diff(Operation.EQUAL, "The-c"),
                new Diff(Operation.INSERT, "ow-and-the-c"),
                new Diff(Operation.EQUAL, "at."),
            ],
            [
                new Diff(Operation.EQUAL, "The-"),
                new Diff(Operation.INSERT, "cow-and-the-"),
                new Diff(Operation.EQUAL, "cat."),
            ]
        );

        yield return (
            "Hitting the start.",
            [
                new Diff(Operation.EQUAL, "a"),
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.EQUAL, "ax"),
            ],
            [
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.EQUAL, "aax"),
            ]
        );

        yield return (
            "Hitting the end.",
            [
                new Diff(Operation.EQUAL, "xa"),
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.EQUAL, "a"),
            ],
            [
                new Diff(Operation.EQUAL, "xaa"),
                new Diff(Operation.DELETE, "a"),
            ]
        );

        yield return (
            "Sentence boundaries.",
            [
                new Diff(Operation.EQUAL, "The xxx. The "),
                new Diff(Operation.INSERT, "zzz. The "),
                new Diff(Operation.EQUAL, "yyy."),
            ],
            [
                new Diff(Operation.EQUAL, "The xxx."),
                new Diff(Operation.INSERT, " The zzz."),
                new Diff(Operation.EQUAL, " The yyy."),
            ]
        );
    }

    [Test]
    [MethodDataSource(nameof(CleanupSemanticLosslessDatasource))]
    public async Task CleanupSemanticLosslessTest(string description, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)
    {
        var actualResult = originalDiff.CleanupSemanticLossless();

        await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    }

    public static IEnumerable<(string description, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)> CleanupSemanticDatasource()
    {
        yield return (
            "No elimination #1.",
            [
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "cd"),
                new Diff(Operation.EQUAL, "12"),
                new Diff(Operation.DELETE, "e"),
            ],
            [
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "cd"),
                new Diff(Operation.EQUAL, "12"),
                new Diff(Operation.DELETE, "e"),
            ]
        );

        yield return (
            "No elimination #2.",
            [
                new Diff(Operation.DELETE, "abc"),
                new Diff(Operation.INSERT, "ABC"),
                new Diff(Operation.EQUAL, "1234"),
                new Diff(Operation.DELETE, "wxyz"),
            ],
            [
                new Diff(Operation.DELETE, "abc"),
                new Diff(Operation.INSERT, "ABC"),
                new Diff(Operation.EQUAL, "1234"),
                new Diff(Operation.DELETE, "wxyz"),
            ]
        );

        yield return (
            "Simple elimination.",
            [
                new Diff(Operation.DELETE, "a"),
                new Diff(Operation.EQUAL, "b"),
                new Diff(Operation.DELETE, "c"),
            ],
            [
                new Diff(Operation.DELETE, "abc"),
                new Diff(Operation.INSERT, "b"),
            ]
        );

        yield return (
            "Backpass elimination.",
            [
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.EQUAL, "cd"),
                new Diff(Operation.DELETE, "e"),
                new Diff(Operation.EQUAL, "f"),
                new Diff(Operation.INSERT, "g"),
            ],
            [
                new Diff(Operation.DELETE, "abcdef"),
                new Diff(Operation.INSERT, "cdfg"),
            ]
        );

        yield return (
            "Multiple elimination.",
            [
                new Diff(Operation.INSERT, "1"),
                new Diff(Operation.EQUAL, "A"),
                new Diff(Operation.DELETE, "B"),
                new Diff(Operation.INSERT, "2"),
                new Diff(Operation.EQUAL, "_"),
                new Diff(Operation.INSERT, "1"),
                new Diff(Operation.EQUAL, "A"),
                new Diff(Operation.DELETE, "B"),
                new Diff(Operation.INSERT, "2"),
            ],
            [
                new Diff(Operation.DELETE, "AB_AB"),
                new Diff(Operation.INSERT, "1A2_1A2"),
            ]
        );

        yield return (
            "Word boundaries.",
            [
                new Diff(Operation.EQUAL, "The c"),
                new Diff(Operation.DELETE, "ow and the c"),
                new Diff(Operation.EQUAL, "at."),
            ],
            [
                new Diff(Operation.EQUAL, "The "),
                new Diff(Operation.DELETE, "cow and the "),
                new Diff(Operation.EQUAL, "cat."),
            ]
        );

        yield return (
            "No overlap elimination.",
            [
                new Diff(Operation.DELETE, "abcxx"),
                new Diff(Operation.INSERT, "xxdef"),
            ],
            [
                new Diff(Operation.DELETE, "abcxx"),
                new Diff(Operation.INSERT, "xxdef"),
            ]
        );

        yield return (
            "Overlap elimination.",
            [
                new Diff(Operation.DELETE, "abcxxx"),
                new Diff(Operation.INSERT, "xxxdef"),
            ],
            [
                new Diff(Operation.DELETE, "abc"),
                new Diff(Operation.EQUAL, "xxx"),
                new Diff(Operation.INSERT, "def"),
            ]
        );

        yield return (
            "Reverse overlap elimination.",
            [
                new Diff(Operation.DELETE, "xxxabc"),
                new Diff(Operation.INSERT, "defxxx"),
            ],
            [
                new Diff(Operation.INSERT, "def"),
                new Diff(Operation.EQUAL, "xxx"),
                new Diff(Operation.DELETE, "abc"),
            ]
        );

        yield return (
            "Two overlap eliminations.",
            [
                new Diff(Operation.DELETE, "abcd1212"),
                new Diff(Operation.INSERT, "1212efghi"),
                new Diff(Operation.EQUAL, "----"),
                new Diff(Operation.DELETE, "A3"),
                new Diff(Operation.INSERT, "3BC"),
            ],
            [
                new Diff(Operation.DELETE, "abcd"),
                new Diff(Operation.EQUAL, "1212"),
                new Diff(Operation.INSERT, "efghi"),
                new Diff(Operation.EQUAL, "----"),
                new Diff(Operation.DELETE, "A"),
                new Diff(Operation.EQUAL, "3"),
                new Diff(Operation.INSERT, "BC"),
            ]
        );
    }

    [Test]
    [MethodDataSource(nameof(CleanupSemanticDatasource))]
    public async Task CleanupSemanticTest(string description, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)
    {
        var actualResult = originalDiff.CleanupSemantic();

        await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    }

    public static IEnumerable<(string description, short editCost, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)> CleanupEfficiencyDatasource()
    {
        yield return (
            "Null case.",
            4,
            [],
            []
        );

        yield return (
            "No elimination.",
            4,
            [
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "wxyz"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "34"),
            ],
            [
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "wxyz"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "34"),
            ]
        );

        yield return (
            "Four-edit elimination.",
            4,
            [
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "xyz"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "34"),
            ],
            [
                new Diff(Operation.DELETE, "abxyzcd"),
                new Diff(Operation.INSERT, "12xyz34"),
            ]
        );

        yield return (
            "Three-edit elimination.",
            4,
            [
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "x"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "34"),
            ],
            [
                new Diff(Operation.DELETE, "xcd"),
                new Diff(Operation.INSERT, "12x34"),
            ]
        );

        yield return (
            "Backpass elimination.",
            4,
            [
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "xy"),
                new Diff(Operation.INSERT, "34"),
                new Diff(Operation.EQUAL, "z"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "56"),
            ],
            [
                new Diff(Operation.DELETE, "abxyzcd"),
                new Diff(Operation.INSERT, "12xy34z56"),
            ]
        );

        yield return (
            "High cost elimination.",
            5,
            [
                new Diff(Operation.DELETE, "ab"),
                new Diff(Operation.INSERT, "12"),
                new Diff(Operation.EQUAL, "wxyz"),
                new Diff(Operation.DELETE, "cd"),
                new Diff(Operation.INSERT, "34"),
            ],
            [
                new Diff(Operation.DELETE, "abwxyzcd"),
                new Diff(Operation.INSERT, "12wxyz34"),
            ]
        );
    }

    [Test]
    [MethodDataSource(nameof(CleanupEfficiencyDatasource))]
    public async Task CleanupEfficiencyTest(string description, short editCost, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)
    {
        var actualResult = originalDiff.CleanupEfficiency(editCost);

        await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    }

    // public void PrettyHtmlTest()
    // {
    //     // Pretty print.
    //     List<Diff> diffs = new List<Diff> {
    //     new Diff(Operation.EQUAL, "a\n"),
    //     new Diff(Operation.DELETE, "<B>b</B>"),
    //     new Diff(Operation.INSERT, "c&d")};
    //     assertEquals("diff_prettyHtml:", "<span>a&para;<br></span><del style=\"background:#ffe6e6;\">&lt;B&gt;b&lt;/B&gt;</del><ins style=\"background:#e6ffe6;\">c&amp;d</ins>",
    //         this.diff_prettyHtml(diffs));
    // }

    // public void TextTest()
    // {
    //     // Compute the source and destination texts.
    //     List<Diff> diffs = new List<Diff> {
    //     new Diff(Operation.EQUAL, "jump"),
    //     new Diff(Operation.DELETE, "s"),
    //     new Diff(Operation.INSERT, "ed"),
    //     new Diff(Operation.EQUAL, " over "),
    //     new Diff(Operation.DELETE, "the"),
    //     new Diff(Operation.INSERT, "a"),
    //     new Diff(Operation.EQUAL, " lazy")};
    //     assertEquals("diff_text1:", "jumps over the lazy", this.diff_text1(diffs));

    //     assertEquals("diff_text2:", "jumped over a lazy", this.diff_text2(diffs));
    // }

    // public void DeltaTest()
    // {
    //     // Convert a diff into delta string.
    //     List<Diff> diffs = new List<Diff> {
    //     new Diff(Operation.EQUAL, "jump"),
    //     new Diff(Operation.DELETE, "s"),
    //     new Diff(Operation.INSERT, "ed"),
    //     new Diff(Operation.EQUAL, " over "),
    //     new Diff(Operation.DELETE, "the"),
    //     new Diff(Operation.INSERT, "a"),
    //     new Diff(Operation.EQUAL, " lazy"),
    //     new Diff(Operation.INSERT, "old dog")};
    //     string text1 = this.diff_text1(diffs);
    //     assertEquals("diff_text1: Base text.", "jumps over the lazy", text1);

    //     string delta = this.diff_toDelta(diffs);
    //     assertEquals("diff_toDelta:", "=4\t-1\t+ed\t=6\t-3\t+a\t=5\t+old dog", delta);

    //     // Convert delta string into a diff.
    //     assertEquals("diff_fromDelta: Normal.", diffs, this.diff_fromDelta(text1, delta));

    //     // Generates error (19 < 20).
    //     try
    //     {
    //         this.diff_fromDelta(text1 + "x", delta);
    //         assertFail("diff_fromDelta: Too long.");
    //     }
    //     catch (ArgumentException)
    //     {
    //         // Exception expected.
    //     }

    //     // Generates error (19 > 18).
    //     try
    //     {
    //         this.diff_fromDelta(text1.Substring(1), delta);
    //         assertFail("diff_fromDelta: Too short.");
    //     }
    //     catch (ArgumentException)
    //     {
    //         // Exception expected.
    //     }

    //     // Generates error (%c3%xy invalid Unicode).
    //     try
    //     {
    //         this.diff_fromDelta("", "+%c3%xy");
    //         assertFail("diff_fromDelta: Invalid character.");
    //     }
    //     catch (ArgumentException)
    //     {
    //         // Exception expected.
    //     }

    //     // Test deltas with special characters.
    //     char zero = (char)0;
    //     char one = (char)1;
    //     char two = (char)2;
    //     diffs = new List<Diff> {
    //     new Diff(Operation.EQUAL, "\u0680 " + zero + " \t %"),
    //     new Diff(Operation.DELETE, "\u0681 " + one + " \n ^"),
    //     new Diff(Operation.INSERT, "\u0682 " + two + " \\ |")};
    //     text1 = this.diff_text1(diffs);
    //     assertEquals("diff_text1: Unicode text.", "\u0680 " + zero + " \t %\u0681 " + one + " \n ^", text1);

    //     delta = this.diff_toDelta(diffs);
    //     // Lowercase, due to UrlEncode uses lower.
    //     assertEquals("diff_toDelta: Unicode.", "=7\t-7\t+%da%82 %02 %5c %7c", delta);

    //     assertEquals("diff_fromDelta: Unicode.", diffs, this.diff_fromDelta(text1, delta));

    //     // Verify pool of unchanged characters.
    //     diffs = new List<Diff> {
    //     new Diff(Operation.INSERT, "A-Z a-z 0-9 - _ . ! ~ * ' ( ) ; / ? : @ & = + $ , # ")};
    //     string text2 = this.diff_text2(diffs);
    //     assertEquals("diff_text2: Unchanged characters.", "A-Z a-z 0-9 - _ . ! ~ * \' ( ) ; / ? : @ & = + $ , # ", text2);

    //     delta = this.diff_toDelta(diffs);
    //     assertEquals("diff_toDelta: Unchanged characters.", "+A-Z a-z 0-9 - _ . ! ~ * \' ( ) ; / ? : @ & = + $ , # ", delta);

    //     // Convert delta string into a diff.
    //     assertEquals("diff_fromDelta: Unchanged characters.", diffs, this.diff_fromDelta("", delta));

    //     // 160 kb string.
    //     string a = "abcdefghij";
    //     for (int i = 0; i < 14; i++)
    //     {
    //         a += a;
    //     }
    //     diffs = new List<Diff> { new Diff(Operation.INSERT, a) };
    //     delta = this.diff_toDelta(diffs);
    //     assertEquals("diff_toDelta: 160kb string.", "+" + a, delta);

    //     // Convert delta string into a diff.
    //     assertEquals("diff_fromDelta: 160kb string.", diffs, this.diff_fromDelta("", delta));
    // }

    // public void XIndexTest()
    // {
    //     // Translate a location in text1 to text2.
    //     List<Diff> diffs = new List<Diff> {
    //     new Diff(Operation.DELETE, "a"),
    //     new Diff(Operation.INSERT, "1234"),
    //     new Diff(Operation.EQUAL, "xyz")};
    //     assertEquals("diff_xIndex: Translation on equality.", 5, this.diff_xIndex(diffs, 2));

    //     diffs = new List<Diff> {
    //     new Diff(Operation.EQUAL, "a"),
    //     new Diff(Operation.DELETE, "1234"),
    //     new Diff(Operation.EQUAL, "xyz")};
    //     assertEquals("diff_xIndex: Translation on deletion.", 1, this.diff_xIndex(diffs, 3));
    // }

    // public void LevenshteinTest()
    // {
    //     List<Diff> diffs = new List<Diff> {
    //     new Diff(Operation.DELETE, "abc"),
    //     new Diff(Operation.INSERT, "1234"),
    //     new Diff(Operation.EQUAL, "xyz")};
    //     assertEquals("diff_levenshtein: Levenshtein with trailing equality.", 4, this.diff_levenshtein(diffs));

    //     diffs = new List<Diff> {
    //     new Diff(Operation.EQUAL, "xyz"),
    //     new Diff(Operation.DELETE, "abc"),
    //     new Diff(Operation.INSERT, "1234")};
    //     assertEquals("diff_levenshtein: Levenshtein with leading equality.", 4, this.diff_levenshtein(diffs));

    //     diffs = new List<Diff> {
    //     new Diff(Operation.DELETE, "abc"),
    //     new Diff(Operation.EQUAL, "xyz"),
    //     new Diff(Operation.INSERT, "1234")};
    //     assertEquals("diff_levenshtein: Levenshtein with middle equality.", 7, this.diff_levenshtein(diffs));
    // }

    // public void BisectTest()
    // {
    //     // Normal.
    //     string a = "cat";
    //     string b = "map";
    //     // Since the resulting diff hasn't been normalized, it would be ok if
    //     // the insertion and deletion pairs are swapped.
    //     // If the order changes, tweak this test as required.
    //     using var normalCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromDays(1));
    //     List<Diff> diffs = new List<Diff> { new Diff(Operation.DELETE, "c"), new Diff(Operation.INSERT, "m"), new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "t"), new Diff(Operation.INSERT, "p") };
    //     assertEquals("diff_bisect: Normal.", diffs, this.diff_bisect(a, b, normalCancellationTokenSource.Token));

    //     // Timeout.
    //     using var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMicroseconds(0));
    //     diffs = new List<Diff> { new Diff(Operation.DELETE, "cat"), new Diff(Operation.INSERT, "map") };
    //     assertEquals("diff_bisect: Timeout.", diffs, this.diff_bisect(a, b, timeoutCancellationTokenSource.Token));
    // }

    // public void MainTest()
    // {
    //     // Perform a trivial diff.
    //     List<Diff> diffs = new List<Diff> { };
    //     assertEquals("diff_main: Null case.", diffs.ToList(), this.diff_main("", "", false).ToList());

    //     // diffs = new List<Diff> {new Diff(Operation.EQUAL, "abc")};
    //     // assertEquals("diff_main: Equality.", diffs, this.diff_main("abc", "abc", false));

    //     // diffs = new List<Diff> {new Diff(Operation.EQUAL, "ab"), new Diff(Operation.INSERT, "123"), new Diff(Operation.EQUAL, "c")};
    //     // assertEquals("diff_main: Simple insertion.", diffs, this.diff_main("abc", "ab123c", false));

    //     // diffs = new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "123"), new Diff(Operation.EQUAL, "bc")};
    //     // assertEquals("diff_main: Simple deletion.", diffs, this.diff_main("a123bc", "abc", false));

    //     // diffs = new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.INSERT, "123"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.INSERT, "456"), new Diff(Operation.EQUAL, "c")};
    //     // assertEquals("diff_main: Two insertions.", diffs, this.diff_main("abc", "a123b456c", false));

    //     // diffs = new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "123"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.DELETE, "456"), new Diff(Operation.EQUAL, "c")};
    //     // assertEquals("diff_main: Two deletions.", diffs, this.diff_main("a123b456c", "abc", false));

    //     // Perform a real diff.
    //     // Switch off the timeout.
    //     DiffTimeout = null;
    //     // diffs = new List<Diff> {new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "b")};
    //     // assertEquals("diff_main: Simple case #1.", diffs, this.diff_main("a", "b", false));

    //     // diffs = new List<Diff> {new Diff(Operation.DELETE, "Apple"), new Diff(Operation.INSERT, "Banana"), new Diff(Operation.EQUAL, "s are a"), new Diff(Operation.INSERT, "lso"), new Diff(Operation.EQUAL, " fruit.")};
    //     // assertEquals("diff_main: Simple case #2.", diffs, this.diff_main("Apples are a fruit.", "Bananas are also fruit.", false));

    //     // diffs = new List<Diff> {new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "\u0680"), new Diff(Operation.EQUAL, "x"), new Diff(Operation.DELETE, "\t"), new Diff(Operation.INSERT, new string (new char[]{(char)0}))};
    //     // assertEquals("diff_main: Simple case #3.", diffs, this.diff_main("ax\t", "\u0680x" + (char)0, false));

    //     // diffs = new List<Diff> {new Diff(Operation.DELETE, "1"), new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "y"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.DELETE, "2"), new Diff(Operation.INSERT, "xab")};
    //     // assertEquals("diff_main: Overlap #1.", diffs, this.diff_main("1ayb2", "abxab", false));

    //     // diffs = new List<Diff> {new Diff(Operation.INSERT, "xaxcx"), new Diff(Operation.EQUAL, "abc"), new Diff(Operation.DELETE, "y")};
    //     // assertEquals("diff_main: Overlap #2.", diffs, this.diff_main("abcy", "xaxcxabc", false));

    //     // diffs = new List<Diff> {new Diff(Operation.DELETE, "ABCD"), new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "="), new Diff(Operation.INSERT, "-"), new Diff(Operation.EQUAL, "bcd"), new Diff(Operation.DELETE, "="), new Diff(Operation.INSERT, "-"), new Diff(Operation.EQUAL, "efghijklmnopqrs"), new Diff(Operation.DELETE, "EFGHIJKLMNOefg")};
    //     // assertEquals("diff_main: Overlap #3.", diffs, this.diff_main("ABCDa=bcd=efghijklmnopqrsEFGHIJKLMNOefg", "a-bcd-efghijklmnopqrs", false));

    //     // diffs = new List<Diff> {new Diff(Operation.INSERT, " "), new Diff(Operation.EQUAL, "a"), new Diff(Operation.INSERT, "nd"), new Diff(Operation.EQUAL, " [[Pennsylvania]]"), new Diff(Operation.DELETE, " and [[New")};
    //     // assertEquals("diff_main: Large equality.", diffs, this.diff_main("a [[Pennsylvania]] and [[New", " and [[Pennsylvania]]", false));

    //     DiffTimeout = TimeSpan.FromMilliseconds(100);
    //     string a = "`Twas brillig, and the slithy toves\nDid gyre and gimble in the wabe:\nAll mimsy were the borogoves,\nAnd the mome raths outgrabe.\n";
    //     string b = "I am the very model of a modern major general,\nI've information vegetable, animal, and mineral,\nI know the kings of England, and I quote the fights historical,\nFrom Marathon to Waterloo, in order categorical.\n";
    //     // Increase the text lengths by 1024 times to ensure a timeout.
    //     for (int i = 0; i < 10; i++)
    //     {
    //         a += a;
    //         b += b;
    //     }
    //     DateTime startTime = DateTime.Now;
    //     this.diff_main(a, b);
    //     DateTime endTime = DateTime.Now;
    //     // Test that we took at least the timeout period.
    //     // assertTrue("diff_main: Timeout min.", new TimeSpan(((long)DiffTimeout.Value.TotalMilliseconds) * 10000) <= endTime - startTime);
    //     // // Test that we didn't take forever (be forgiving).
    //     // // Theoretically this test could fail very occasionally if the
    //     // // OS task swaps or locks up for a second at the wrong moment.
    //     // assertTrue("diff_main: Timeout max.", new TimeSpan(((long)DiffTimeout.Value.TotalMilliseconds) * 10000 * 2) > endTime - startTime);
    //     DiffTimeout = null;

    //     // Test the linemode speedup.
    //     // Must be long to pass the 100 char cutoff.
    //     // a = "1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n";
    //     // b = "abcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\n";
    //     // assertEquals("diff_main: Simple line-mode.", this.diff_main(a, b, true), this.diff_main(a, b, false));

    //     // a = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
    //     // b = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghij";
    //     // assertEquals("diff_main: Single line-mode.", this.diff_main(a, b, true), this.diff_main(a, b, false));

    //     // a = "1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n";
    //     // b = "abcdefghij\n1234567890\n1234567890\n1234567890\nabcdefghij\n1234567890\n1234567890\n1234567890\nabcdefghij\n1234567890\n1234567890\n1234567890\nabcdefghij\n";
    //     // string[] texts_linemode = diff_rebuildtexts(this.diff_main(a, b, true));
    //     // string[] texts_textmode = diff_rebuildtexts(this.diff_main(a, b, false));
    //     // assertEquals("diff_main: Overlap line-mode.", texts_textmode, texts_linemode);

    //     // Test null inputs -- not needed because nulls can't be passed in C#.
    // }
}
