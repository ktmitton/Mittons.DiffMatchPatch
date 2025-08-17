namespace Mittons.DiffMatchPatch.Extensions;

public static class ReadOnlySpanExtensions
{
    /// <summary>
    /// Attempts to find the longest common prefix between two texts.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="other"></param>
    /// <param name="commonPrefix"></param>
    /// <returns>True if there is a common prefix, false otherwise.</returns>
    public static bool TryFindCommonPrefix(this ReadOnlySpan<char> @this, ReadOnlySpan<char> other, out ReadOnlySpan<char> commonPrefix)
    {
        commonPrefix = @this[..@this.CommonPrefixLength(other)];

        return commonPrefix.Length > 0;
    }

    public static bool TryFindCommonPrefix(this string @this, string other, out ReadOnlySpan<char> commonPrefix)
        => @this.AsSpan().TryFindCommonPrefix(other, out commonPrefix);

    /// <summary>
    /// Attempts to find the longest common suffix between two texts.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="other"></param>
    /// <param name="commonSuffix"></param>
    /// <returns>True if there is a common suffix, false otherwise.</returns>
    public static bool TryFindCommonSuffix(this ReadOnlySpan<char> @this, ReadOnlySpan<char> other, out ReadOnlySpan<char> commonSuffix)
    {
        var maximumSharedLength = Math.Min(@this.Length, other.Length);

        commonSuffix = [];

        for (int i = 1; i <= maximumSharedLength; ++i)
        {
            if (@this[^i] != other[^i])
            {
                break;
            }

            commonSuffix = @this[^i..];
        }

        return commonSuffix.Length > 0;
    }

    public static bool TryFindCommonSuffix(this string @this, string other, out ReadOnlySpan<char> commonSuffix)
        => @this.AsSpan().TryFindCommonSuffix(other, out commonSuffix);

    public static bool TryFindCommonHalfMiddle(
        this string @this,
        string other,
        out ReadOnlySpan<char> leftPrefix,
        out ReadOnlySpan<char> leftSuffix,
        out ReadOnlySpan<char> rightPrefix,
        out ReadOnlySpan<char> rightSuffix,
        out ReadOnlySpan<char> commonMiddle
    ) => @this.AsSpan().TryFindCommonHalfMiddle(
            other,
            out leftPrefix,
            out leftSuffix,
            out rightPrefix,
            out rightSuffix,
            out commonMiddle
        );

    /// <summary>
    /// Attempts to find the longest common string within two texts that is at least half the length of the longer text.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="other"></param>
    /// <param name="commonPrefix"></param>
    /// <returns></returns>
    public static bool TryFindCommonHalfMiddle(
        this ReadOnlySpan<char> @this,
        ReadOnlySpan<char> other,
        out ReadOnlySpan<char> leftPrefix,
        out ReadOnlySpan<char> leftSuffix,
        out ReadOnlySpan<char> rightPrefix,
        out ReadOnlySpan<char> rightSuffix,
        out ReadOnlySpan<char> commonMiddle
    )
    {
        var swapSides = @this.Length <= other.Length;
        var longerText = swapSides ? other : @this;
        var shorterText = swapSides ? @this : other;

        if ((longerText.Length < 4) || ((shorterText.Length * 2) < longerText.Length))
        {
            leftPrefix = leftSuffix = rightPrefix = rightSuffix = commonMiddle = [];
            return false;
        }

        var hasSecondQuarterMatch = TryFindCommonHalfMiddleFromIndex(
            longerText,
            shorterText,
            (longerText.Length + 3) / 4,
            out var secondQuarterLeftPrefix,
            out var secondQuarterLeftSuffix,
            out var secondQuarterRightPrefix,
            out var secondQuarterRightSuffix,
            out var secondQuarterCommonMiddle
        );

        var hasThirdQuarterMatch = TryFindCommonHalfMiddleFromIndex(
            longerText,
            shorterText,
            (longerText.Length + 1) / 2,
            out var thirdQuarterLeftPrefix,
            out var thirdQuarterLeftSuffix,
            out var thirdQuarterRightPrefix,
            out var thirdQuarterRightSuffix,
            out var thirdQuarterCommonMiddle
        );

        // First check if the second quarter is the seed for a half-match.
        if (!hasSecondQuarterMatch && !hasThirdQuarterMatch)
        {
            leftPrefix = leftSuffix = rightPrefix = rightSuffix = commonMiddle = [];

            return false;
        }

        var useSecondQuarter = secondQuarterCommonMiddle.Length > thirdQuarterCommonMiddle.Length;

        if (swapSides && useSecondQuarter)
        {
            leftPrefix = secondQuarterRightPrefix;
            leftSuffix = secondQuarterRightSuffix;
            rightPrefix = secondQuarterLeftPrefix;
            rightSuffix = secondQuarterLeftSuffix;
            commonMiddle = secondQuarterCommonMiddle;
        }
        else if (swapSides)
        {
            leftPrefix = thirdQuarterRightPrefix;
            leftSuffix = thirdQuarterRightSuffix;
            rightPrefix = thirdQuarterLeftPrefix;
            rightSuffix = thirdQuarterLeftSuffix;
            commonMiddle = thirdQuarterCommonMiddle;
        }
        else if (useSecondQuarter)
        {
            leftPrefix = secondQuarterLeftPrefix;
            leftSuffix = secondQuarterLeftSuffix;
            rightPrefix = secondQuarterRightPrefix;
            rightSuffix = secondQuarterRightSuffix;
            commonMiddle = secondQuarterCommonMiddle;
        }
        else
        {
            leftPrefix = thirdQuarterLeftPrefix;
            leftSuffix = thirdQuarterLeftSuffix;
            rightPrefix = thirdQuarterRightPrefix;
            rightSuffix = thirdQuarterRightSuffix;
            commonMiddle = thirdQuarterCommonMiddle;
        }

        return true;
    }

    /// <summary>
    /// Attempts to find the longest common string in the middle of two texts that is at least half the size of the longest text.
    /// </summary>
    /// <param name="longerText">The longer of the two texts being compared.</param>
    /// <param name="shorterText">The shorter of the two texts being compared.</param>
    /// <param name="startIndex">The start index of the quarter used as the search term.</param>
    /// <returns></returns>
    /// <remarks>
    /// When the a quarter is searched, the search term is 1/4 the length of the longest text, starting at the beginning of the quarter.
    /// The shortest text is then searched for all occurences of that search term, filtered to the result that has the most shared
    /// characters on either side of the search result. If the longest result is less than half the length of the longest text, nothing
    /// is returned.
    /// <para>
    /// The commonSuffixLength and commonPrefixLength seem a little counter intuitive since they call CommonPrefixLength and
    /// CommonSuffixLength respectively. What's happening is the CommonPrefixLength function is called on the text after the matched
    /// searched term, and the CommonSuffixLength is called on the text before the matched search term.
    /// </para>
    /// <para>
    /// For example, let's say the search term was 567, and the texts were 123456789012 and 023456780. We'd run the CommonPrefixLength
    /// on 89012 and 80, with 8 being the only common character, this would be our suffix for the final common string. CommonSuffixLength
    /// would be run on 1234 and 0234, with 234 being the common characters, which are the prefix for the final common string. This means
    /// the final result is 234 + 567 + 8, so 2345678.
    /// </para>
    /// </remarks>
    private static bool TryFindCommonHalfMiddleFromIndex(
        ReadOnlySpan<char> longerText,
        ReadOnlySpan<char> shorterText,
        int startIndex,
        out ReadOnlySpan<char> leftPrefix,
        out ReadOnlySpan<char> leftSuffix,
        out ReadOnlySpan<char> rightPrefix,
        out ReadOnlySpan<char> rightSuffix,
        out ReadOnlySpan<char> commonMiddle
    )
    {
        var searchTerm = longerText.Slice(startIndex, longerText.Length / 4);

        leftPrefix = leftSuffix = rightPrefix = rightSuffix = commonMiddle = [];

        for (int i = shorterText.IndexOf(searchTerm); i != -1; i = shorterText.IndexOf(searchTerm, i + 1))
        {
            longerText[startIndex..].TryFindCommonPrefix(shorterText[i..], out var commonSuffix);
            longerText[..startIndex].TryFindCommonSuffix(shorterText[..i], out var commonPrefix);

            if (commonMiddle.Length < commonPrefix.Length + commonSuffix.Length)
            {
                commonMiddle = shorterText.Slice(i - commonPrefix.Length, commonPrefix.Length + commonSuffix.Length);
                leftPrefix = longerText[..(startIndex - commonPrefix.Length)];
                leftSuffix = longerText[(startIndex + commonSuffix.Length)..];
                rightPrefix = shorterText[..(i - commonPrefix.Length)];
                rightSuffix = shorterText[(i + commonSuffix.Length)..];
            }
        }

        if (commonMiddle.Length * 2 < longerText.Length)
        {
            leftPrefix = leftSuffix = rightPrefix = rightSuffix = commonMiddle = [];

            return false;
        }

        return true;
    }

    public static bool TryFindCommonOverlap(
        this ReadOnlySpan<char> @this,
        ReadOnlySpan<char> other,
        out ReadOnlySpan<char> commonOverlap
    )
    {
        commonOverlap = [];

        var maximumOverlap = Math.Min(@this.Length, other.Length);
        if (maximumOverlap == 0)
        {
            return false;
        }

        var thisSpan = @this[^maximumOverlap..];
        var otherSpan = other[..maximumOverlap];

        if (thisSpan.SequenceEqual(otherSpan))
        {
            commonOverlap = thisSpan;

            return true;
        }

        for (int i = maximumOverlap - 1; i > -1; --i)
        {
            var pattern = thisSpan[i..];
            if (otherSpan.StartsWith(pattern))
            {
                commonOverlap = pattern;

                continue;
            }

            if (otherSpan.IndexOf(pattern, StringComparison.Ordinal) == -1)
            {
                break;
            }
        }

        return false;
    }
}
