using System.Text;
using Mittons.DiffMatchPatch.Extensions;
using Mittons.DiffMatchPatch.Models;
using Mittons.DiffMatchPatch.Types;

namespace Mittons.DiffMatchPatch.Test;

public class DiffTest
{
    // public record HashTextsTestData(string LeftText, string RightText, HashedTexts? HashedTexts);

    // public static IEnumerable<Func<HashTextsTestData>> HashTextsDatasource()
    // {
    //     yield return () => new HashTextsTestData(
    //         "alpha\nbeta\nalpha\n",
    //         "beta\nalpha\nbeta\n",
    //         new HashedTexts(
    //             ["alpha\n", "beta\n"],
    //             "\0\u0001\0",
    //             "\u0001\0\u0001"
    //         )
    //     );

    //     yield return () => new HashTextsTestData(
    //         "",
    //         "alpha\r\nbeta\r\n\r\n\r\n",
    //         new HashedTexts(
    //             ["alpha\r\n", "beta\r\n", "\r\n"],
    //             "",
    //             "\0\u0001\u0002\u0002"
    //         )
    //     );

    //     yield return () => new HashTextsTestData(
    //         "a",
    //         "b",
    //         new HashedTexts(
    //             ["a", "b"],
    //             "\0",
    //             "\u0001"
    //         )
    //     );

    //     yield return () => new HashTextsTestData(
    //         "",
    //         string.Join("", Enumerable.Range(0, 300).Select(x => $"{x}\n")),
    //         new HashedTexts(
    //             [.. Enumerable.Range(0, 300).Select(x => $"{x}\n")],
    //             "",
    //             string.Join("", Enumerable.Range(0, 300).Select(x => $"{(char)x}"))
    //         )
    //     );
    // }

    // [Test]
    // [MethodDataSource(nameof(HashTextsDatasource))]
    // public async Task LinesToCharsTest(HashTextsTestData testData)
    // {
    //     var actualHashedTexts = testData.LeftText.CompressTexts(testData.RightText);

    //     var a = actualHashedTexts.HashLookup;
    //     var b = testData.HashedTexts?.HashLookup;
    //     await Assert.That(b).IsEquivalentTo(a);
    //     await Assert.That(actualHashedTexts).IsEquivalentTo(testData.HashedTexts);
    // }

    // public static IEnumerable<(IList<string> lookup, IEnumerable<Diff> encryptedDiff, IEnumerable<Diff> expectedResult)> DecompressDiffDatasource()
    // {
    //     yield return (
    //         [
    //             "alpha\n",
    //             "beta\n",
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "\0\u0001\0"),
    //             new Diff(Operation.INSERT, "\u0001\0\u0001"),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "alpha\nbeta\nalpha\n"),
    //             new Diff(Operation.INSERT, "beta\nalpha\nbeta\n"),
    //         ]
    //     );

    //     yield return (
    //         Enumerable.Range(0, 300).Select(x => $"{x}\n").ToList(),
    //         [
    //             new Diff(Operation.DELETE, string.Join("", Enumerable.Range(0, 300).Select(x => (char)x)))
    //         ],
    //         [
    //             new Diff(Operation.DELETE, string.Join("", Enumerable.Range(0, 300).Select(x => $"{x}\n")))
    //         ]
    //     );

    //     var originalText = string.Join("", Enumerable.Range(0, 66000).Select(x => $"{x}\n"));
    //     List<string> decompressionLookup = [];
    //     Dictionary<string, int> compressionLookup = [];
    //     var compressedLines = Extensions.StringExtensions.CompressLines(originalText, decompressionLookup, compressionLookup, 4);
    //     yield return (
    //         decompressionLookup,
    //         [
    //             new Diff(Operation.INSERT, string.Join("", compressedLines))
    //         ],
    //         [
    //             new Diff(Operation.INSERT, originalText)
    //         ]
    //     );
    // }

    // [Test]
    // [MethodDataSource(nameof(DecompressDiffDatasource))]
    // public async Task DecompressDiffTest(IList<string> lookup, IEnumerable<Diff> encryptedDiff, IEnumerable<Diff> expectedResult)
    // {
    //     var actualResult = encryptedDiff.DecompressDiff(lookup);

    //     await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    // }

    // public static IEnumerable<(IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)> CleanupMergeDatasource()
    // {
    //     yield return (
    //         [
    //             new Diff(Operation.EQUAL, "a"),
    //             new Diff(Operation.DELETE, "b"),
    //             new Diff(Operation.INSERT, "c"),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "a"),
    //             new Diff(Operation.DELETE, "b"),
    //             new Diff(Operation.INSERT, "c"),
    //         ]
    //     );

    //     foreach (var operation in new Operation[] { Operation.EQUAL, Operation.DELETE, Operation.INSERT })
    //     {
    //         yield return (
    //             [
    //                 new Diff(operation, "a"),
    //                 new Diff(operation, "b"),
    //                 new Diff(operation, "c"),
    //             ],
    //             [
    //                 new Diff(operation, "abc"),
    //             ]
    //         );
    //     }

    //     yield return (
    //         [
    //             new Diff(Operation.DELETE, "a"),
    //             new Diff(Operation.INSERT, "b"),
    //             new Diff(Operation.DELETE, "c"),
    //             new Diff(Operation.INSERT, "d"),
    //             new Diff(Operation.EQUAL, "e"),
    //             new Diff(Operation.EQUAL, "f"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "ac"),
    //             new Diff(Operation.INSERT, "bd"),
    //             new Diff(Operation.EQUAL, "ef"),
    //         ]
    //     );

    //     yield return (
    //         [
    //             new Diff(Operation.DELETE, "a"),
    //             new Diff(Operation.INSERT, "abc"),
    //             new Diff(Operation.DELETE, "dc"),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "a"),
    //             new Diff(Operation.DELETE, "d"),
    //             new Diff(Operation.INSERT, "b"),
    //             new Diff(Operation.EQUAL, "c"),
    //         ]
    //     );

    //     yield return (
    //         [
    //             new Diff(Operation.EQUAL, "x"),
    //             new Diff(Operation.DELETE, "a"),
    //             new Diff(Operation.INSERT, "abc"),
    //             new Diff(Operation.DELETE, "dc"),
    //             new Diff(Operation.EQUAL, "y"),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "xa"),
    //             new Diff(Operation.DELETE, "d"),
    //             new Diff(Operation.INSERT, "b"),
    //             new Diff(Operation.EQUAL, "cy"),
    //         ]
    //     );

    //     yield return (
    //         [
    //             new Diff(Operation.EQUAL, "a"),
    //             new Diff(Operation.INSERT, "ba"),
    //             new Diff(Operation.EQUAL, "c"),
    //         ],
    //         [
    //             new Diff(Operation.INSERT, "ab"),
    //             new Diff(Operation.EQUAL, "ac"),
    //         ]
    //     );

    //     yield return (
    //         [
    //             new Diff(Operation.EQUAL, "c"),
    //             new Diff(Operation.INSERT, "ab"),
    //             new Diff(Operation.EQUAL, "a"),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "ca"),
    //             new Diff(Operation.INSERT, "ba"),
    //         ]
    //     );

    //     yield return (
    //         [
    //             new Diff(Operation.DELETE, "b"),
    //             new Diff(Operation.INSERT, "ab"),
    //             new Diff(Operation.EQUAL, "c"),
    //         ],
    //         [
    //             new Diff(Operation.INSERT, "a"),
    //             new Diff(Operation.EQUAL, "bc"),
    //         ]
    //     );

    //     yield return (
    //         [
    //             new Diff(Operation.EQUAL, ""),
    //             new Diff(Operation.INSERT, "a"),
    //             new Diff(Operation.EQUAL, "b"),
    //         ],
    //         [
    //             new Diff(Operation.INSERT, "a"),
    //             new Diff(Operation.EQUAL, "b"),
    //         ]
    //     );
    // }

    // public static IEnumerable<(IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)> CleanupMergeMultipassDatasource()
    // {
    //     yield return (
    //         [
    //             new Diff(Operation.EQUAL, "a"),
    //             new Diff(Operation.DELETE, "b"),
    //             new Diff(Operation.EQUAL, "c"),
    //             new Diff(Operation.DELETE, "ac"),
    //             new Diff(Operation.EQUAL, "x")
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "abc"),
    //             new Diff(Operation.EQUAL, "acx"),
    //         ]
    //     );

    //     yield return (
    //         [
    //             new Diff(Operation.EQUAL, "x"),
    //             new Diff(Operation.DELETE, "ca"),
    //             new Diff(Operation.EQUAL, "c"),
    //             new Diff(Operation.DELETE, "b"),
    //             new Diff(Operation.EQUAL, "a"),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "xca"),
    //             new Diff(Operation.DELETE, "cba"),
    //         ]
    //     );
    // }

    // [Test]
    // [MethodDataSource(nameof(CleanupMergeDatasource))]
    // public async Task CleanupMergeTest(IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)
    // {
    //     var actualResult = originalDiff.CleanupMerge();

    //     await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    // }

    // [Test]
    // [Skip("We currently aren't doing multipass on this since that's only needed if this is called outside the normal workflow. Within the normal workflow, things _should_ be optimized enough that we don't have multiple groups that need checked together.")]
    // [MethodDataSource(nameof(CleanupMergeMultipassDatasource))]
    // public async Task CleanupMergeTest_Multipass(IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)
    // {
    //     var actualResult = originalDiff.CleanupMerge();

    //     await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    // }

    // public static IEnumerable<(string description, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)> CleanupSemanticLosslessDatasource()
    // {
    //     yield return ("Null case.", [], []);

    //     yield return (
    //         "Blank lines.",
    //         [
    //             new Diff(Operation.EQUAL, "AAA\r\n\r\nBBB"),
    //             new Diff(Operation.INSERT, "\r\nDDD\r\n\r\nBBB"),
    //             new Diff(Operation.EQUAL, "\r\nEEE"),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "AAA\r\n\r\n"),
    //             new Diff(Operation.INSERT, "BBB\r\nDDD\r\n\r\n"),
    //             new Diff(Operation.EQUAL, "BBB\r\nEEE"),
    //         ]
    //     );

    //     yield return (
    //         "Line boundaries.",
    //         [
    //             new Diff(Operation.EQUAL, "AAA\r\nBBB"),
    //             new Diff(Operation.INSERT, " DDD\r\nBBB"),
    //             new Diff(Operation.EQUAL, " EEE"),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "AAA\r\n"),
    //             new Diff(Operation.INSERT, "BBB DDD\r\n"),
    //             new Diff(Operation.EQUAL, "BBB EEE"),
    //         ]
    //     );

    //     yield return (
    //         "Word boundaries.",
    //         [
    //             new Diff(Operation.EQUAL, "The c"),
    //             new Diff(Operation.INSERT, "ow and the c"),
    //             new Diff(Operation.EQUAL, "at."),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "The "),
    //             new Diff(Operation.INSERT, "cow and the "),
    //             new Diff(Operation.EQUAL, "cat."),
    //         ]
    //     );

    //     yield return (
    //         "Alphanumeric boundaries.",
    //         [
    //             new Diff(Operation.EQUAL, "The-c"),
    //             new Diff(Operation.INSERT, "ow-and-the-c"),
    //             new Diff(Operation.EQUAL, "at."),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "The-"),
    //             new Diff(Operation.INSERT, "cow-and-the-"),
    //             new Diff(Operation.EQUAL, "cat."),
    //         ]
    //     );

    //     yield return (
    //         "Hitting the start.",
    //         [
    //             new Diff(Operation.EQUAL, "a"),
    //             new Diff(Operation.DELETE, "a"),
    //             new Diff(Operation.EQUAL, "ax"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "a"),
    //             new Diff(Operation.EQUAL, "aax"),
    //         ]
    //     );

    //     yield return (
    //         "Hitting the end.",
    //         [
    //             new Diff(Operation.EQUAL, "xa"),
    //             new Diff(Operation.DELETE, "a"),
    //             new Diff(Operation.EQUAL, "a"),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "xaa"),
    //             new Diff(Operation.DELETE, "a"),
    //         ]
    //     );

    //     yield return (
    //         "Sentence boundaries.",
    //         [
    //             new Diff(Operation.EQUAL, "The xxx. The "),
    //             new Diff(Operation.INSERT, "zzz. The "),
    //             new Diff(Operation.EQUAL, "yyy."),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "The xxx."),
    //             new Diff(Operation.INSERT, " The zzz."),
    //             new Diff(Operation.EQUAL, " The yyy."),
    //         ]
    //     );
    // }

    // [Test]
    // [MethodDataSource(nameof(CleanupSemanticLosslessDatasource))]
    // public async Task CleanupSemanticLosslessTest(string description, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)
    // {
    //     var actualResult = originalDiff.CleanupSemanticLossless();

    //     await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    // }

    // public static IEnumerable<(string description, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)> CleanupSemanticDatasource()
    // {
    //     yield return (
    //         "No elimination #1.",
    //         [
    //             new Diff(Operation.DELETE, "ab"),
    //             new Diff(Operation.INSERT, "cd"),
    //             new Diff(Operation.EQUAL, "12"),
    //             new Diff(Operation.DELETE, "e"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "ab"),
    //             new Diff(Operation.INSERT, "cd"),
    //             new Diff(Operation.EQUAL, "12"),
    //             new Diff(Operation.DELETE, "e"),
    //         ]
    //     );

    //     yield return (
    //         "No elimination #2.",
    //         [
    //             new Diff(Operation.DELETE, "abc"),
    //             new Diff(Operation.INSERT, "ABC"),
    //             new Diff(Operation.EQUAL, "1234"),
    //             new Diff(Operation.DELETE, "wxyz"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "abc"),
    //             new Diff(Operation.INSERT, "ABC"),
    //             new Diff(Operation.EQUAL, "1234"),
    //             new Diff(Operation.DELETE, "wxyz"),
    //         ]
    //     );

    //     yield return (
    //         "Simple elimination.",
    //         [
    //             new Diff(Operation.DELETE, "a"),
    //             new Diff(Operation.EQUAL, "b"),
    //             new Diff(Operation.DELETE, "c"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "abc"),
    //             new Diff(Operation.INSERT, "b"),
    //         ]
    //     );

    //     yield return (
    //         "Backpass elimination.",
    //         [
    //             new Diff(Operation.DELETE, "ab"),
    //             new Diff(Operation.EQUAL, "cd"),
    //             new Diff(Operation.DELETE, "e"),
    //             new Diff(Operation.EQUAL, "f"),
    //             new Diff(Operation.INSERT, "g"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "abcdef"),
    //             new Diff(Operation.INSERT, "cdfg"),
    //         ]
    //     );

    //     yield return (
    //         "Multiple elimination.",
    //         [
    //             new Diff(Operation.INSERT, "1"),
    //             new Diff(Operation.EQUAL, "A"),
    //             new Diff(Operation.DELETE, "B"),
    //             new Diff(Operation.INSERT, "2"),
    //             new Diff(Operation.EQUAL, "_"),
    //             new Diff(Operation.INSERT, "1"),
    //             new Diff(Operation.EQUAL, "A"),
    //             new Diff(Operation.DELETE, "B"),
    //             new Diff(Operation.INSERT, "2"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "AB_AB"),
    //             new Diff(Operation.INSERT, "1A2_1A2"),
    //         ]
    //     );

    //     yield return (
    //         "Word boundaries.",
    //         [
    //             new Diff(Operation.EQUAL, "The c"),
    //             new Diff(Operation.DELETE, "ow and the c"),
    //             new Diff(Operation.EQUAL, "at."),
    //         ],
    //         [
    //             new Diff(Operation.EQUAL, "The "),
    //             new Diff(Operation.DELETE, "cow and the "),
    //             new Diff(Operation.EQUAL, "cat."),
    //         ]
    //     );

    //     yield return (
    //         "No overlap elimination.",
    //         [
    //             new Diff(Operation.DELETE, "abcxx"),
    //             new Diff(Operation.INSERT, "xxdef"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "abcxx"),
    //             new Diff(Operation.INSERT, "xxdef"),
    //         ]
    //     );

    //     yield return (
    //         "Overlap elimination.",
    //         [
    //             new Diff(Operation.DELETE, "abcxxx"),
    //             new Diff(Operation.INSERT, "xxxdef"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "abc"),
    //             new Diff(Operation.EQUAL, "xxx"),
    //             new Diff(Operation.INSERT, "def"),
    //         ]
    //     );

    //     yield return (
    //         "Reverse overlap elimination.",
    //         [
    //             new Diff(Operation.DELETE, "xxxabc"),
    //             new Diff(Operation.INSERT, "defxxx"),
    //         ],
    //         [
    //             new Diff(Operation.INSERT, "def"),
    //             new Diff(Operation.EQUAL, "xxx"),
    //             new Diff(Operation.DELETE, "abc"),
    //         ]
    //     );

    //     yield return (
    //         "Two overlap eliminations.",
    //         [
    //             new Diff(Operation.DELETE, "abcd1212"),
    //             new Diff(Operation.INSERT, "1212efghi"),
    //             new Diff(Operation.EQUAL, "----"),
    //             new Diff(Operation.DELETE, "A3"),
    //             new Diff(Operation.INSERT, "3BC"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "abcd"),
    //             new Diff(Operation.EQUAL, "1212"),
    //             new Diff(Operation.INSERT, "efghi"),
    //             new Diff(Operation.EQUAL, "----"),
    //             new Diff(Operation.DELETE, "A"),
    //             new Diff(Operation.EQUAL, "3"),
    //             new Diff(Operation.INSERT, "BC"),
    //         ]
    //     );
    // }

    // [Test]
    // [MethodDataSource(nameof(CleanupSemanticDatasource))]
    // public async Task CleanupSemanticTest(string description, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)
    // {
    //     var actualResult = originalDiff.CleanupSemantic();

    //     await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    // }

    // public static IEnumerable<(string description, short editCost, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)> CleanupEfficiencyDatasource()
    // {
    //     yield return (
    //         "Null case.",
    //         4,
    //         [],
    //         []
    //     );

    //     yield return (
    //         "No elimination.",
    //         4,
    //         [
    //             new Diff(Operation.DELETE, "ab"),
    //             new Diff(Operation.INSERT, "12"),
    //             new Diff(Operation.EQUAL, "wxyz"),
    //             new Diff(Operation.DELETE, "cd"),
    //             new Diff(Operation.INSERT, "34"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "ab"),
    //             new Diff(Operation.INSERT, "12"),
    //             new Diff(Operation.EQUAL, "wxyz"),
    //             new Diff(Operation.DELETE, "cd"),
    //             new Diff(Operation.INSERT, "34"),
    //         ]
    //     );

    //     yield return (
    //         "Four-edit elimination.",
    //         4,
    //         [
    //             new Diff(Operation.DELETE, "ab"),
    //             new Diff(Operation.INSERT, "12"),
    //             new Diff(Operation.EQUAL, "xyz"),
    //             new Diff(Operation.DELETE, "cd"),
    //             new Diff(Operation.INSERT, "34"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "abxyzcd"),
    //             new Diff(Operation.INSERT, "12xyz34"),
    //         ]
    //     );

    //     yield return (
    //         "Three-edit elimination.",
    //         4,
    //         [
    //             new Diff(Operation.INSERT, "12"),
    //             new Diff(Operation.EQUAL, "x"),
    //             new Diff(Operation.DELETE, "cd"),
    //             new Diff(Operation.INSERT, "34"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "xcd"),
    //             new Diff(Operation.INSERT, "12x34"),
    //         ]
    //     );

    //     yield return (
    //         "Backpass elimination.",
    //         4,
    //         [
    //             new Diff(Operation.DELETE, "ab"),
    //             new Diff(Operation.INSERT, "12"),
    //             new Diff(Operation.EQUAL, "xy"),
    //             new Diff(Operation.INSERT, "34"),
    //             new Diff(Operation.EQUAL, "z"),
    //             new Diff(Operation.DELETE, "cd"),
    //             new Diff(Operation.INSERT, "56"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "abxyzcd"),
    //             new Diff(Operation.INSERT, "12xy34z56"),
    //         ]
    //     );

    //     yield return (
    //         "High cost elimination.",
    //         5,
    //         [
    //             new Diff(Operation.DELETE, "ab"),
    //             new Diff(Operation.INSERT, "12"),
    //             new Diff(Operation.EQUAL, "wxyz"),
    //             new Diff(Operation.DELETE, "cd"),
    //             new Diff(Operation.INSERT, "34"),
    //         ],
    //         [
    //             new Diff(Operation.DELETE, "abwxyzcd"),
    //             new Diff(Operation.INSERT, "12wxyz34"),
    //         ]
    //     );
    // }

    // [Test]
    // [MethodDataSource(nameof(CleanupEfficiencyDatasource))]
    // public async Task CleanupEfficiencyTest(string description, short editCost, IEnumerable<Diff> originalDiff, IEnumerable<Diff> expectedResult)
    // {
    //     var actualResult = originalDiff.CleanupEfficiency(editCost);

    //     await Assert.That(actualResult).IsEquivalentTo(expectedResult);
    // }

    // [Test]
    // public async Task PrettyHtmlTest()
    // {
    //     var expectedResult = """<span>a&para;<br></span><del style="background:#ffe6e6;">&lt;B&gt;b&lt;/B&gt;</del><ins style="background:#e6ffe6;">c&amp;d</ins>""";

    //     Diff[] diffs = [
    //         new Diff(Operation.EQUAL, "a\n"),
    //         new Diff(Operation.DELETE, "<B>b</B>"),
    //         new Diff(Operation.INSERT, "c&d"),
    //     ];

    //     var actualResult = diffs.CreateHtmlReport();

    //     await Assert.That(actualResult).IsEqualTo(expectedResult);
    // }

    // public static IEnumerable<(IEnumerable<Diff> diffs, string expectedOriginalText)> ComposeOriginalTextDatasource()
    // {
    //     yield return (
    //         [
    //             new Diff(Operation.EQUAL, "jump"),
    //             new Diff(Operation.DELETE, "s"),
    //             new Diff(Operation.INSERT, "ed"),
    //             new Diff(Operation.EQUAL, " over "),
    //             new Diff(Operation.DELETE, "the"),
    //             new Diff(Operation.INSERT, "a"),
    //             new Diff(Operation.EQUAL, " lazy"),
    //         ],
    //         "jumps over the lazy"
    //     );

    //     yield return (
    //         [
    //             new Diff(Operation.EQUAL, $"\u0680 {(char)0} \t %"),
    //             new Diff(Operation.DELETE, $"\u0681 {(char)1} \n ^"),
    //             new Diff(Operation.INSERT, $"\u0682 {(char)2} \\ |"),
    //         ],
    //         $"\u0680 {(char)0} \t %\u0681 {(char)1} \n ^"
    //     );
    // }

    // [Test]
    // [MethodDataSource(nameof(ComposeOriginalTextDatasource))]
    // public async Task ComposeOriginalTextTest(IEnumerable<Diff> diffs, string expectedOriginalText)
    // {
    //     var actualOriginalText = diffs.ComposeOriginalText();

    //     await Assert.That(actualOriginalText).IsEqualTo(expectedOriginalText);
    // }

    // [Test]
    // public async Task ComposeFinalTextTest()
    // {
    //     var expectedFinalText = "jumped over a lazy";

    //     // Compute the source and destination texts.
    //     Diff[] diffs = [
    //         new Diff(Operation.EQUAL, "jump"),
    //         new Diff(Operation.DELETE, "s"),
    //         new Diff(Operation.INSERT, "ed"),
    //         new Diff(Operation.EQUAL, " over "),
    //         new Diff(Operation.DELETE, "the"),
    //         new Diff(Operation.INSERT, "a"),
    //         new Diff(Operation.EQUAL, " lazy"),
    //     ];

    //     var actualFinalText = diffs.ComposeFinalText();

    //     await Assert.That(actualFinalText).IsEqualTo(expectedFinalText);
    // }

    // [Test]
    // public async Task ToDeltaEncodedStringTest()
    // {
    //     var expectedDelta = "=4\t-1\t+ed\t=6\t-3\t+a\t=5\t+old dog";

    //     Diff[] diffs = [
    //         new Diff(Operation.EQUAL, "jump"),
    //         new Diff(Operation.DELETE, "s"),
    //         new Diff(Operation.INSERT, "ed"),
    //         new Diff(Operation.EQUAL, " over "),
    //         new Diff(Operation.DELETE, "the"),
    //         new Diff(Operation.INSERT, "a"),
    //         new Diff(Operation.EQUAL, " lazy"),
    //         new Diff(Operation.INSERT, "old dog"),
    //     ];

    //     var actualDelta = diffs.ToDeltaEncodedString();

    //     await Assert.That(actualDelta).IsEqualTo(expectedDelta);
    // }

    // [Test]
    // public async Task FromDeltaEncodedStringTest()
    // {
    //     var deltaEncodedString = "=4\t-1\t+ed\t=6\t-3\t+a\t=5\t+old dog";
    //     var sourceText = "jumps over the lazy";

    //     Diff[] expectedDiffs = [
    //         new Diff(Operation.EQUAL, "jump"),
    //         new Diff(Operation.DELETE, "s"),
    //         new Diff(Operation.INSERT, "ed"),
    //         new Diff(Operation.EQUAL, " over "),
    //         new Diff(Operation.DELETE, "the"),
    //         new Diff(Operation.INSERT, "a"),
    //         new Diff(Operation.EQUAL, " lazy"),
    //         new Diff(Operation.INSERT, "old dog"),
    //     ];

    //     var actualDiff = sourceText.ToDiff(deltaEncodedString);

    //     await Assert.That(actualDiff).IsEquivalentTo(expectedDiffs);
    // }

    // [Test]
    // public void FromDeltaEncodedStringFailTest()
    // {
    //     var deltaEncodedString = "=4\t-1\t+ed\t=6\t-3\t+a\t=5\t+old dog";
    //     var sourceText = "jumps over the lazyx";

    //     Assert.Throws<ArgumentException>(() => sourceText.ToDiff(deltaEncodedString).ToArray());
    // }

    // [Test]
    // public void FromDeltaEncodedStringFailTest2()
    // {
    //     var deltaEncodedString = "=4\t-1\t+ed\t=6\t-3\t+a\t=5\t+old dog";
    //     var sourceText = "umps over the lazy";

    //     Assert.Throws<ArgumentException>(() => sourceText.ToDiff(deltaEncodedString).ToArray());
    // }

    // [Test]
    // [Skip(".net is too lenient for this to matter, gotta rethink it")]
    // public void FromDeltaEncodedStringFailTest3()
    // {
    //     var deltaEncodedString = "+%c3%xy";
    //     var sourceText = "";

    //     Assert.Throws<ArgumentException>(() => sourceText.ToDiff(deltaEncodedString).ToArray());
    // }

    // [Test]
    // public async Task ToDeltaEncodedStringSpecialCharactersTest()
    // {
    //     Diff[] diffs = [
    //         new Diff(Operation.EQUAL, $"\u0680 {(char)0} \t %"),
    //         new Diff(Operation.DELETE, $"\u0681 {(char)1} \n ^"),
    //         new Diff(Operation.INSERT, $"\u0682 {(char)2} \\ |"),
    //     ];

    //     var expectedDeltaEncodeString = "=7\t-7\t+%da%82 %02 %5c %7c";

    //     var actualDeltaEncodeString = diffs.ToDeltaEncodedString();

    //     await Assert.That(actualDeltaEncodeString).IsEqualTo(expectedDeltaEncodeString);
    // }

    // [Test]
    // public async Task FromDeltaEncodedStringSpecialCharactersTest()
    // {
    //     var deltaEncodedString = "=7\t-7\t+%da%82 %02 %5c %7c";
    //     var sourceText = $"\u0680 {(char)0} \t %\u0681 {(char)1} \n ^";

    //     Diff[] expectedDiffs = [
    //         new Diff(Operation.EQUAL, $"\u0680 {(char)0} \t %"),
    //         new Diff(Operation.DELETE, $"\u0681 {(char)1} \n ^"),
    //         new Diff(Operation.INSERT, $"\u0682 {(char)2} \\ |"),
    //     ];

    //     var actualDiffs = sourceText.ToDiff(deltaEncodedString);

    //     await Assert.That(actualDiffs).IsEquivalentTo(expectedDiffs);
    // }

    // [Test]
    // public async Task UnchangedCharacterTest()
    // {
    //     var originalText = string.Empty;
    //     Diff[] expectedDiffs = [
    //         new Diff(Operation.INSERT, "A-Z a-z 0-9 - _ . ! ~ * ' ( ) ; / ? : @ & = + $ , # "),
    //     ];
    //     var expectedFinalText = "A-Z a-z 0-9 - _ . ! ~ * ' ( ) ; / ? : @ & = + $ , # ";
    //     var expectedDeltaEncodeString = "+A-Z a-z 0-9 - _ . ! ~ * \' ( ) ; / ? : @ & = + $ , # ";

    //     var actualFinalText = expectedDiffs.ComposeFinalText();
    //     var actualDeltaEncodeString = expectedDiffs.ToDeltaEncodedString();
    //     var actualDiffs = originalText.ToDiff(actualDeltaEncodeString);

    //     await Assert.That(actualFinalText).IsEqualTo(expectedFinalText);
    //     await Assert.That(actualDeltaEncodeString).IsEqualTo(expectedDeltaEncodeString);
    //     await Assert.That(actualDiffs).IsEquivalentTo(expectedDiffs);
    // }

    // [Test]
    // public async Task LongStringTest()
    // {
    //     var originalText = string.Empty;
    //     var expectedFinalText = string.Join("", Enumerable.Range(0, 14).Select(_ => "abcdefghij"));
    //     var expectedDeltaEncodeString = $"+{expectedFinalText}";
    //     Diff[] expectedDiffs = [
    //         new Diff(Operation.INSERT, expectedFinalText),
    //     ];

    //     var actualFinalText = expectedDiffs.ComposeFinalText();
    //     var actualDeltaEncodeString = expectedDiffs.ToDeltaEncodedString();
    //     var actualDiffs = originalText.ToDiff(actualDeltaEncodeString);

    //     await Assert.That(actualFinalText).IsEqualTo(expectedFinalText);
    //     await Assert.That(actualDeltaEncodeString).IsEqualTo(expectedDeltaEncodeString);
    //     await Assert.That(actualDiffs).IsEquivalentTo(expectedDiffs);
    // }

    // [Test]
    // public async Task ValidLocationInBothTexts()
    // {
    //     // Translate a location in text1 to text2.
    //     Diff[] diffs = [
    //         new Diff(Operation.DELETE, "a"),
    //         new Diff(Operation.INSERT, "1234"),
    //         new Diff(Operation.EQUAL, "xyz"),
    //     ];
    //     var expectedFinalLocation = 5;
    //     var originalLocation = 2;

    //     var actualFinalLocation = diffs.ConvertLocationInOriginalTextToLocationInFinalText(originalLocation);

    //     await Assert.That(actualFinalLocation).IsEqualTo(expectedFinalLocation);
    // }

    // [Test]
    // public async Task DeletedLocationInFinalText()
    // {
    //     Diff[] diffs = [
    //         new Diff(Operation.EQUAL, "a"),
    //         new Diff(Operation.DELETE, "1234"),
    //         new Diff(Operation.EQUAL, "xyz"),
    //     ];
    //     var expectedFinalLocation = 1;
    //     var originalLocation = 3;

    //     var actualFinalLocation = diffs.ConvertLocationInOriginalTextToLocationInFinalText(originalLocation);

    //     await Assert.That(actualFinalLocation).IsEqualTo(expectedFinalLocation);
    // }

    // [Test]
    // [MethodDataSource(nameof(LevenshteinDatasource))]
    // public async Task LevenshteinTest(string description, IEnumerable<Diff> diffs, int expectedDistance)
    // {
    //     var actualDistance = diffs.CalculateLevenshteinDistance();

    //     await Assert.That(actualDistance).IsEqualTo(expectedDistance);
    // }

    // public static IEnumerable<(string description, IEnumerable<Diff> diffs, int expectedDistance)> LevenshteinDatasource()
    // {
    //     yield return (
    //         "Trailing equality",
    //         [
    //             new Diff(Operation.DELETE, "abc"),
    //             new Diff(Operation.INSERT, "1234"),
    //             new Diff(Operation.EQUAL, "xyz"),
    //         ],
    //         4
    //     );

    //     yield return (
    //         "Leading equality",
    //         [
    //             new Diff(Operation.EQUAL, "xyz"),
    //             new Diff(Operation.DELETE, "abc"),
    //             new Diff(Operation.INSERT, "1234"),
    //         ],
    //         4
    //     );

    //     yield return (
    //         "Middle equality",
    //         [
    //             new Diff(Operation.DELETE, "abc"),
    //             new Diff(Operation.EQUAL, "xyz"),
    //             new Diff(Operation.INSERT, "1234"),
    //         ],
    //         7
    //     );
    // }

    // [Test]
    // public async Task BisectTest()
    // {
    //     // Normal.
    //     string a = "cat";
    //     string b = "map";
    //     // Since the resulting diff hasn't been normalized, it would be ok if
    //     // the insertion and deletion pairs are swapped.
    //     // If the order changes, tweak this test as required.
    //     using var normalCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromDays(1));
    //     Diff[] diffs = [
    //         new Diff(Operation.DELETE, "c"),
    //         new Diff(Operation.INSERT, "m"),
    //         new Diff(Operation.EQUAL, "a"),
    //         new Diff(Operation.DELETE, "t"),
    //         new Diff(Operation.INSERT, "p"),
    //     ];
    //     var actualDiffs = a.BisectTexts(b, normalCancellationTokenSource.Token);
    //     await Assert.That(actualDiffs).IsEquivalentTo(diffs);
    //     // assertEquals("diff_bisect: Normal.", diffs, this.diff_bisect(a, b, normalCancellationTokenSource.Token));

    //     // Timeout.
    //     // using var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMicroseconds(0));
    //     // diffs = new List<Diff> { new Diff(Operation.DELETE, "cat"), new Diff(Operation.INSERT, "map") };
    //     // assertEquals("diff_bisect: Timeout.", diffs, this.diff_bisect(a, b, timeoutCancellationTokenSource.Token));
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
