using Mittons.DiffMatchPatch.Extensions;

namespace Mittons.DiffMatchPatch.Test.Extensions;

public class ReadOnlySpanExtensionsTests
{
    [Test]
    [Arguments("abc", "xyz", "")]
    [Arguments("1234abcdef", "1234xyz", "1234")]
    [Arguments("1234", "1234xyz", "1234")]
    public async Task TryCommonPrefixLength(string originalText, string finalText, string expectedCommonPrefix)
    {
        var expectedWasPrefixFound = expectedCommonPrefix.Length > 0;

        var actualWasPrefixFound = originalText.TryFindCommonPrefix(finalText, out var actualCommonPrefixSpan);
        var actualCommonPrefix = actualCommonPrefixSpan.ToString();

        await Assert.That(actualWasPrefixFound).IsEqualTo(expectedWasPrefixFound);
        await Assert.That(actualCommonPrefix).IsEqualTo(expectedCommonPrefix);
    }

    [Test]
    [Arguments("abc", "xyz", "")]
    [Arguments("abcdef1234", "xyz1234", "1234")]
    [Arguments("1234", "xyz1234", "1234")]
    public async Task TryCommonSuffixLength(string originalText, string finalText, string expectedCommonSuffix)
    {
        var expectedWasSuffixFound = expectedCommonSuffix.Length > 0;

        var actualWasSuffixFound = originalText.TryFindCommonSuffix(finalText, out var actualCommonSuffixSpan);
        var actualCommonSuffix = actualCommonSuffixSpan.ToString();

        await Assert.That(actualWasSuffixFound).IsEqualTo(expectedWasSuffixFound);
        await Assert.That(actualCommonSuffix).IsEqualTo(expectedCommonSuffix);
    }

    [Test]
    [Arguments("1234567890", "abcdef")]
    [Arguments("12345", "23")]
    public async Task CommonMiddle_WhenThereIsNoCommonMiddle_ExpectEmptyDetails(string left, string right)
    {
        var expectedWasMiddleFound = false;

        var actualWasMiddleFound = left.TryFindCommonHalfMiddle(right, out _, out _, out _, out _, out _);

        await Assert.That(actualWasMiddleFound).IsEqualTo(expectedWasMiddleFound);
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
    public async Task CommonMiddle_WhenThereIsACommonMiddle_ExpectDetails(
        string left,
        string right,
        string expectedLeftPrefix,
        string expectedLeftSuffix,
        string expectedRightPrefix,
        string expectedRightSuffix,
        string expectedCommonMiddle
    )
    {
        var expectedWasMiddleFound = true;

        var actualWasMiddleFound = left.TryFindCommonHalfMiddle(
            right,
            out var actualLeftPrefixSpan,
            out var actualLeftSuffixSpan,
            out var actualRightPrefixSpan,
            out var actualRightSuffixSpan,
            out var actualCommonMiddleSpan
        );

        var actualLeftPrefix = actualLeftPrefixSpan.ToString();
        var actualLeftSuffix = actualLeftSuffixSpan.ToString();
        var actualRightPrefix = actualRightPrefixSpan.ToString();
        var actualRightSuffix = actualRightSuffixSpan.ToString();
        var actualCommonMiddle = actualCommonMiddleSpan.ToString();

        await Assert.That(actualWasMiddleFound).IsEqualTo(expectedWasMiddleFound);

        await Assert.That(actualLeftPrefix).IsEqualTo(expectedLeftPrefix);
        await Assert.That(actualLeftSuffix).IsEqualTo(expectedLeftSuffix);
        await Assert.That(actualRightPrefix).IsEqualTo(expectedRightPrefix);
        await Assert.That(actualRightSuffix).IsEqualTo(expectedRightSuffix);
        await Assert.That(actualCommonMiddle).IsEqualTo(expectedCommonMiddle);
    }

    [Test]
    [Arguments("qHilloHelloHew", "xHelloHeHulloy")]
    [Skip("We should be checking the cancellation token in the caller, not here")]
    public async Task CommonMiddle_WhenThereNoCancellationToken_ExpectNoDetails(string left, string right)
    {
        var expectedWasMiddleFound = false;

        var actualWasMiddleFound = left.TryFindCommonHalfMiddle(right, out _, out _, out _, out _, out _);

        await Assert.That(actualWasMiddleFound).IsEqualTo(expectedWasMiddleFound);
    }

    [Test]
    [Arguments("", "abcd")]
    [Arguments("123456", "abcd")]
    [Arguments("abcd", "")]
    [Arguments("abcd", "123456")]
    public async Task CommonOverlapLength_WhenThereIsNoOverlap_ExpectNoCommonality(string left, string right)
    {
        var expectedWasOverlapFound = false;

        var actualWasOverlapFound = left.AsSpan().TryFindCommonOverlap(right, out _, out _, out _, out _, out _);

        await Assert.That(actualWasOverlapFound).IsEqualTo(expectedWasOverlapFound);
    }

    [Test]
    [Arguments("fi", "\ufb01i")]
    [Arguments("\ufb01i", "fi")]
    public async Task CommonOverlapLength_WhenOverlapMixesUnicodeLigaturesWithAscii_ExpectNoCommonality(
        string left,
        string right
    )
    {
        var expectedWasOverlapFound = false;

        var actualWasOverlapFound = left.AsSpan().TryFindCommonOverlap(right, out _, out _, out _, out _, out _);

        await Assert.That(actualWasOverlapFound).IsEqualTo(expectedWasOverlapFound);
    }

    [Test]
    [Arguments("abc", "abcd", "", "", "", "d", "abc")]
    [Arguments("123456xxx", "xxxabcd", "123456", "", "", "abcd", "xxx")]
    [Arguments("dabc", "abc", "d", "", "", "", "abc")]
    [Arguments("abcdxxx", "xxx123456", "abcd", "", "", "123456", "xxx")]
    [Arguments("fi", "i\ufb01", "f", "", "", "\ufb01", "i")]
    [Arguments("i\ufb01", "fi", "", "\ufb01", "f", "", "i")]
    public async Task CommonOverlapLength_WhenThereIsOverlap_ExpectCommonality(
        string left,
        string right,
        string expectedLeftPrefix,
        string expectedLeftSuffix,
        string expectedRightPrefix,
        string expectedRightSuffix,
        string expectedCommonOverlap
    )
    {
        var expectedWasOverlapFound = true;

        var actualWasOverlapFound = left.TryFindCommonOverlap(right, out var leftPrefixSpan, out var leftSuffixSpan, out var rightPrefixSpan, out var rightSuffixSpan, out var commonOverlapSpan);
        var actualLeftPrefix = leftPrefixSpan.ToString();
        var actualLeftSuffix = leftSuffixSpan.ToString();
        var actualRightPrefix = rightPrefixSpan.ToString();
        var actualRightSuffix = rightSuffixSpan.ToString();
        var actualCommonOverlap = commonOverlapSpan.ToString();

        await Assert.That(actualWasOverlapFound).IsEqualTo(expectedWasOverlapFound);
        await Assert.That(actualLeftPrefix).IsEqualTo(expectedLeftPrefix);
        await Assert.That(actualLeftSuffix).IsEqualTo(expectedLeftSuffix);
        await Assert.That(actualRightPrefix).IsEqualTo(expectedRightPrefix);
        await Assert.That(actualRightSuffix).IsEqualTo(expectedRightSuffix);
        await Assert.That(actualCommonOverlap).IsEqualTo(expectedCommonOverlap);
    }
}
