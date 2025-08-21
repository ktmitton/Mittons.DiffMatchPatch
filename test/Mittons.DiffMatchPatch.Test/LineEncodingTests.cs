using System.Text;
using Mittons.DiffMatchPatch.Extensions;
using Mittons.DiffMatchPatch.Models;
using Mittons.DiffMatchPatch.Types;

namespace Mittons.DiffMatchPatch.Test;

public class LineEncodingTests
{
    public record HashTextsTestData(string LeftText, string RightText, HashedTexts? HashedTexts);

    public static IEnumerable<Func<HashTextsTestData>> HashTextsDatasource()
    {
        yield return () => new HashTextsTestData(
            "alpha\nbeta\nalpha\n",
            "beta\nalpha\nbeta\n",
            new HashedTexts(
                ["alpha\n", "beta\n"],
                "\0\u0001\0",
                "\u0001\0\u0001"
            )
        );

        yield return () => new HashTextsTestData(
            "",
            "alpha\r\nbeta\r\n\r\n\r\n",
            new HashedTexts(
                ["alpha\r\n", "beta\r\n", "\r\n"],
                "",
                "\0\u0001\u0002\u0002"
            )
        );

        yield return () => new HashTextsTestData(
            "a",
            "b",
            new HashedTexts(
                ["a", "b"],
                "\0",
                "\u0001"
            )
        );

        yield return () => new HashTextsTestData(
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
    public async Task LinesToCharsTest(HashTextsTestData testData)
    {
        var actualHashedTexts = testData.LeftText.CompressTexts(testData.RightText);

        // var a = actualHashedTexts.HashLookup;
        // var b = testData.HashedTexts?.HashLookup;
        await Assert.That(actualHashedTexts.HashLookup).IsEquivalentTo(testData.HashedTexts?.HashLookup);
        await Assert.That(actualHashedTexts).IsEquivalentTo(testData.HashedTexts);
    }
}
