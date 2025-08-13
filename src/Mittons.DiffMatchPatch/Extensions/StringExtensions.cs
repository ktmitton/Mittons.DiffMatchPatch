using System.Text;
using System.Text.RegularExpressions;
using Mittons.DiffMatchPatch.Models;
using Mittons.DiffMatchPatch.Types;

namespace Mittons.DiffMatchPatch.Extensions;

public static class StringExtensions
{
    public static int IndexOf(this ReadOnlySpan<char> @this, ReadOnlySpan<char> value, int startIndex)
    {
        var newIndex = @this[startIndex..].IndexOf(value);

        return newIndex == -1 ? -1 : newIndex + startIndex;
    }

    public static int CommonOverlapLength(this ReadOnlySpan<char> @this, ReadOnlySpan<char> other)
    {
        int maximumOverlap = Math.Min(@this.Length, other.Length);

        if (maximumOverlap == 0)
        {
            return 0;
        }

        var thisSpan = @this[^maximumOverlap..];
        var otherSpan = other[..maximumOverlap];

        if (thisSpan.SequenceEqual(otherSpan))
        {
            return maximumOverlap;
        }

        var best = 0;
        for (int i = maximumOverlap - 1; i > -1; --i)
        {
            if (otherSpan.StartsWith(thisSpan[i..]))
            {
                best = i;

                continue;
            }

            if (thisSpan.IndexOf(otherSpan[..(i + 1)]) == -1)
            {
                break;
            }
        }

        return best == 0 ? 0 : maximumOverlap - best;
    }

    public static int CommonSuffixLength(this ReadOnlySpan<char> @this, ReadOnlySpan<char> other)
    {
        var maximumSharedLength = Math.Min(@this.Length, other.Length);

        for (int i = 1; i <= maximumSharedLength; ++i)
        {
            if (@this[^i] != other[^i])
            {
                return i - 1;
            }
        }

        return maximumSharedLength;
    }

    public static int CommonPrefixLength(this string @this, string other)
        => @this.AsSpan().CommonPrefixLength(other.AsSpan());

    public static int CommonOverlapLength(this string @this, string other)
        => @this.AsSpan().CommonOverlapLength(other);

    public static int CommonSuffixLength(this string @this, string other)
        => @this.AsSpan().CommonSuffixLength(other);

    /// <summary>
    /// Finds the largest string shared between two texts that is at least half the length of the longest text.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="other"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// When you break the longest text into quarters, the variations spanning the fewest quarters are:
    /// <list type="bullet">
    /// <item>q1->q2</item>
    /// <item>q2->q3</item>
    /// <item>q3->q4</item>
    /// </list>
    /// This means that we only need to search starting with q2 and q3, any overlaps into q1 and q4 will naturally
    /// be picked up.
    /// <para>
    /// </para>
    /// </remarks>
    public static CommonMiddleDetails CommonMiddle(this string @this, string other, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            return new();
        }

        var (longtext, shorttext) = @this.Length > other.Length ? (@this, other) : (other, @this);
        if ((longtext.Length < 4) || ((shorttext.Length * 2) < longtext.Length))
        {
            return new();
        }

        // First check if the second quarter is the seed for a half-match.
        var secondQuarterCommonMiddle = CommonMiddleFromIndex(longtext, shorttext, (longtext.Length + 3) / 4);
        var thirdQuarterCommonMiddle = CommonMiddleFromIndex(longtext, shorttext, (longtext.Length + 1) / 2);

        var commonMiddleDetails = secondQuarterCommonMiddle > thirdQuarterCommonMiddle ? secondQuarterCommonMiddle : thirdQuarterCommonMiddle;

        if (commonMiddleDetails.CommonMiddle.Length == 0)
        {
            return new();
        }

        if (@this.Length > other.Length)
        {
            return commonMiddleDetails;
        }

        return new(commonMiddleDetails.RightPrefix, commonMiddleDetails.RightSuffix, commonMiddleDetails.LeftPrefix, commonMiddleDetails.LeftSuffix, commonMiddleDetails.CommonMiddle);
    }
    /**
        * Does a Substring of shorttext exist within longtext such that the
        * Substring is at least half the length of longtext?
        * @param longtext Longer string.
        * @param shorttext Shorter string.
        * @param i Start index of quarter length Substring within longtext.
        * @return Five element string array, containing the prefix of longtext, the
        *     suffix of longtext, the prefix of shorttext, the suffix of shorttext
        *     and the common middle.  Or null if there was no match.
        */
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
    private static CommonMiddleDetails CommonMiddleFromIndex(ReadOnlySpan<char> longerText, ReadOnlySpan<char> shorterText, int startIndex)
    {
        var searchTerm = longerText.Slice(startIndex, longerText.Length / 4);

        ReadOnlySpan<char> bestCommonString = [];
        ReadOnlySpan<char> uniquePrefixToLongestText = [];
        ReadOnlySpan<char> uniqueSuffixToLongestText = [];
        ReadOnlySpan<char> uniquePrefixToShortestText = [];
        ReadOnlySpan<char> uniqueSuffixToShortestText = [];

        for (int i = shorterText.IndexOf(searchTerm); i != -1; i = shorterText.IndexOf(searchTerm, i + 1))
        {
            int commonSuffixLength = longerText[startIndex..].CommonPrefixLength(shorterText[i..]);
            int commonPrefixLength = longerText[..startIndex].CommonSuffixLength(shorterText[..i]);

            if (bestCommonString.Length < commonPrefixLength + commonSuffixLength)
            {
                bestCommonString = shorterText.Slice(i - commonPrefixLength, commonPrefixLength + commonSuffixLength);
                uniquePrefixToLongestText = longerText[..(startIndex - commonPrefixLength)];
                uniqueSuffixToLongestText = longerText[(startIndex + commonSuffixLength)..];
                uniquePrefixToShortestText = shorterText[..(i - commonPrefixLength)];
                uniqueSuffixToShortestText = shorterText[(i + commonSuffixLength)..];
            }
        }

        if (bestCommonString.Length * 2 < longerText.Length)
        {
            return new();
        }

        return new CommonMiddleDetails(uniquePrefixToLongestText, uniqueSuffixToLongestText, uniquePrefixToShortestText, uniqueSuffixToShortestText, bestCommonString);
    }

    /**
     * Split two texts into a list of strings.  Reduce the texts to a string of
     * hashes where each Unicode character represents one line.
     * @param text1 First string.
     * @param text2 Second string.
     * @return Three element Object array, containing the encoded text1, the
     *     encoded text2 and the List of unique strings.  The zeroth element
     *     of the List of unique strings is intentionally blank.
     */
    public static HashedTexts CompressTexts(this string text1, string text2)
    {
        List<string> lineArray = [];
        Dictionary<string, int> lineHash = [];

        // Allocate 2/3rds of the space for text1, the rest for text2.
        IEnumerable<char> chars1 = CompressLines(text1, lineArray, lineHash, 40000);
        IEnumerable<char> chars2 = CompressLines(text2, lineArray, lineHash, 65536);

        return new(lineArray, new string([.. chars1]), new string([.. chars2]));
    }

    /**
     * Split a text into a list of strings.  Reduce the texts to a string of
     * hashes where each Unicode character represents one line.
     * @param text String to encode.
     * @param lineArray List of unique strings.
     * @param lineHash Map of strings to indices.
     * @param maxLines Maximum length of lineArray.
     * @return Encoded string.
     */
    public static IEnumerable<char> CompressLines(string text, List<string> lineArray, Dictionary<string, int> lineHashes, int maxLines)
    {
        int lineStart = 0;
        int lineEnd = -1;
        string line;

        while ((lineEnd < text.Length - 1) && lineArray.Count < (maxLines + 1))
        {
            lineEnd = text.IndexOf('\n', lineStart);
            if (lineEnd == -1)
            {
                lineEnd = text.Length - 1;
            }
            line = text[lineStart..(lineEnd + 1)];

            if (!lineHashes.TryGetValue(line, out var hash))
            {
                if (lineArray.Count == maxLines)
                {
                    line = text[lineStart..];
                    lineEnd = text.Length;
                }
                hash = lineArray.Count;
                lineHashes[line] = hash;
                lineArray.Add(line);
            }

            lineStart = lineEnd + 1;

            yield return (char)hash;
        }
    }

    /**
        * Rehydrate the text in a diff from a string of line hashes to real lines
        * of text.
        * @param diffs List of Diff objects.
        * @param lineArray List of unique strings.
        */

    public static IEnumerable<Diff> DecompressDiff(this IEnumerable<Diff> @this, IList<string> lookup)
    {
        foreach (var diff in @this)
        {
            yield return diff with { Text = string.Join("", diff.Text.Select(x => lookup[x])) };
        }
    }

    private static (IEnumerable<Diff> OptimizedDiffs, string NextRetainedText) OptimizeSection(
        StringBuilder previousRetainedTextBuilder,
        StringBuilder deletedTextBuilder,
        StringBuilder insertedTextBuilder,
        StringBuilder nextRetainedTextBuilder
    )
    {
        var deletedText = deletedTextBuilder.ToString();
        var insertedText = insertedTextBuilder.ToString();

        var commonLength = deletedText.CommonSuffixLength(insertedText);
        var nextRetainedText = commonLength == 0 ? nextRetainedTextBuilder.ToString() : $"{insertedText[^commonLength..]}{nextRetainedTextBuilder}";
        if (commonLength > 0)
        {
            insertedText = insertedText[..^commonLength];
            deletedText = deletedText[..^commonLength];
        }

        commonLength = deletedText.CommonPrefixLength(insertedText);
        if (commonLength > 0)
        {
            previousRetainedTextBuilder.Append(insertedText[..commonLength]);

            insertedText = insertedText[commonLength..];
            deletedText = deletedText[commonLength..];
        }

        var previousRetainedText = previousRetainedTextBuilder.ToString();

        var hasSingleModifier = string.IsNullOrEmpty(deletedText) != string.IsNullOrEmpty(insertedText);
        var isBoundedByRetainedText = !string.IsNullOrEmpty(previousRetainedText) && !string.IsNullOrEmpty(nextRetainedText);
        var skipOptimization = !hasSingleModifier || !isBoundedByRetainedText;

        if (skipOptimization)
        {
            List<Diff> optimizedDiffs = [];

            if (previousRetainedText.Length > 0)
            {
                optimizedDiffs.Add(new Diff(Operation.EQUAL, previousRetainedText));
            }

            if (deletedText.Length > 0)
            {
                optimizedDiffs.Add(new Diff(Operation.DELETE, deletedText));
            }

            if (insertedText.Length > 0)
            {
                optimizedDiffs.Add(new Diff(Operation.INSERT, insertedText));
            }

            return (optimizedDiffs, nextRetainedText);
        }

        var modifiedOperation = string.IsNullOrEmpty(deletedText) ? Operation.INSERT : Operation.DELETE;
        var modifiedText = string.IsNullOrEmpty(deletedText) ? insertedText : deletedText;

        if (modifiedText.EndsWith(previousRetainedText, StringComparison.Ordinal))
        {
            return (
                [new Diff(modifiedOperation, $"{previousRetainedText}{modifiedText[..previousRetainedText.Length]}")],
                $"{previousRetainedText}{nextRetainedText}"
            );
        }

        if (modifiedText.StartsWith(nextRetainedText, StringComparison.Ordinal))
        {
            return (
                [
                    new Diff(Operation.EQUAL, $"{previousRetainedText}{nextRetainedText}"),
                    new Diff(modifiedOperation, $"{modifiedText[nextRetainedText.Length..]}{nextRetainedText}")
                ],
                string.Empty
            );
        }

        return (
            [new Diff(Operation.EQUAL, previousRetainedText), new Diff(modifiedOperation, modifiedText)],
            nextRetainedText
        );
    }

    public static IEnumerable<Diff> CleanupMerge(this IEnumerable<Diff> diffs)
    {
        Operation[] modifierOperations = [Operation.INSERT, Operation.DELETE];

        StringBuilder deletedTextBuilder = new();
        StringBuilder insertedTextBuilder = new();
        StringBuilder previousRetainedTextBuilder = new();
        StringBuilder nextRetainedTextBuilder = new();

        IEnumerable<Diff> optimizedDiffs;
        string nextRetainedText;

        foreach (var diff in diffs)
        {
            if (string.IsNullOrEmpty(diff.Text))
            {
                continue;
            }

            if (modifierOperations.Contains(diff.Operation) && (nextRetainedTextBuilder.Length > 0))
            {
                (optimizedDiffs, nextRetainedText) = OptimizeSection(
                    previousRetainedTextBuilder,
                    deletedTextBuilder,
                    insertedTextBuilder,
                    nextRetainedTextBuilder
                );

                foreach (var optimizedDiff in optimizedDiffs)
                {
                    yield return optimizedDiff;
                }

                deletedTextBuilder.Clear();
                insertedTextBuilder.Clear();
                previousRetainedTextBuilder.Clear();
                nextRetainedTextBuilder.Clear();

                if (!string.IsNullOrEmpty(nextRetainedText))
                {
                    previousRetainedTextBuilder.Append(nextRetainedText);
                }
            }

            switch (diff.Operation)
            {
                case Operation.INSERT:
                    insertedTextBuilder.Append(diff.Text);

                    break;
                case Operation.DELETE:
                    deletedTextBuilder.Append(diff.Text);

                    break;
                case Operation.EQUAL:
                    if ((insertedTextBuilder.Length == 0) && (deletedTextBuilder.Length == 0))
                    {
                        previousRetainedTextBuilder.Append(diff.Text);
                    }
                    else
                    {
                        nextRetainedTextBuilder.Append(diff.Text);
                    }

                    break;
            }
        }

        (optimizedDiffs, nextRetainedText) = OptimizeSection(
            previousRetainedTextBuilder,
            deletedTextBuilder,
            insertedTextBuilder,
            nextRetainedTextBuilder
        );

        foreach (var optimizedDiff in optimizedDiffs)
        {
            yield return optimizedDiff;
        }

        if (!string.IsNullOrEmpty(nextRetainedText))
        {
            yield return new Diff(Operation.EQUAL, nextRetainedText);
        }
    }

    public static IEnumerable<Diff> CleanupSemanticLossless(this IEnumerable<Diff> diffs)
    {
        List<Diff> previousDiffs = [];
        int commonOffset;

        foreach (var currentDiff in diffs)
        {
            switch (previousDiffs.Count)
            {
                case 0:
                    if (currentDiff.Operation == Operation.EQUAL)
                    {
                        previousDiffs.Add(currentDiff);
                    }

                    continue;
                case 1:
                    previousDiffs.Add(currentDiff);

                    continue;
                case 2:
                    if (currentDiff.Operation != Operation.EQUAL)
                    {
                        yield return previousDiffs[0];
                        yield return previousDiffs[1];

                        previousDiffs.Clear();

                        continue;
                    }

                    break;
            }

            string equality1 = previousDiffs[0].Text;
            string edit = previousDiffs[1].Text;
            string equality2 = currentDiff.Text;

            // First, shift the edit as far left as possible
            commonOffset = equality1.CommonSuffixLength(edit);
            if (commonOffset > 0)
            {
                var commonString = edit[^commonOffset..];
                equality1 = equality1[..^commonOffset];
                edit = $"{commonString}{edit[..^commonOffset]}";
                equality2 = $"{commonString}{equality2}";
            }

            // Second, step character by character right, looking for the best fit
            string bestEquality1 = equality1;
            string bestEdit = edit;
            string bestEquality2 = equality2;
            var bestScore = ComputeSemanticComparisonScore(equality1, edit) + ComputeSemanticComparisonScore(edit, equality2);

            while (edit.Length != 0 && equality2.Length != 0 && edit[0] == equality2[0])
            {
                equality1 += edit[0];
                edit = $"{edit[1..]}{equality2[0]}";
                equality2 = equality2[1..];

                var score = ComputeSemanticComparisonScore(equality1, edit) + ComputeSemanticComparisonScore(edit, equality2);

                // The >= encourages trailing rather than leading whitespace on edits.
                if (score >= bestScore)
                {
                    bestScore = score;
                    bestEquality1 = equality1;
                    bestEdit = edit;
                    bestEquality2 = equality2;
                }
            }

            if (previousDiffs[0].Text == bestEquality1)
            {
                yield return previousDiffs[0];
                yield return previousDiffs[1];
                previousDiffs.Clear();
                previousDiffs.Add(currentDiff);

                continue;
            }

            if (bestEquality1.Length != 0)
            {
                yield return new Diff(Operation.EQUAL, bestEquality1);
            }

            yield return previousDiffs[1] with { Text = bestEdit };

            previousDiffs.Clear();
            if (bestEquality2.Length != 0)
            {
                previousDiffs.Add(new Diff(Operation.EQUAL, bestEquality2));
            }
        }

        foreach (var diff in previousDiffs)
        {
            yield return diff;
        }
    }

    /// <summary>
    /// Given two strings, compute a score representing whether the internal
    /// boundary falls on logical boundaries.
    /// </summary>
    /// <param name="left">The string whose end boundary is being evaluated.</param>
    /// <param name="right">The string whose start boundary is being evaluated.</param>
    /// <returns>The score representing the comparison result.</returns>
    /// <remarks>
    /// Scores range from 0 (worst) to 6 (best):
    /// <list type="number">
    /// <item>No match</item>
    /// <item>Non-alphanumeric</item>
    /// <item>Whitespace</item>
    /// <item>End of sentence</item>
    /// <item>Line break</item>
    /// <item>Blank lines</item>
    /// <item>Edge match, both strings are empty</item>
    /// </list>
    /// </remarks>
    private static int ComputeSemanticComparisonScore(string left, string right)
    {
        Regex BLANKLINEEND = new("\\n\\r?\\n\\Z");
        Regex BLANKLINESTART = new("\\A\\r?\\n\\r?\\n");

        if (left.Length == 0 || right.Length == 0)
        {
            return 6;
        }

        char leftEndingCharacter = left[^1];
        char rightStartingCharacter = right[0];

        bool leftEndsWithAlphaNumeric = char.IsLetterOrDigit(leftEndingCharacter);
        bool rightStartsWithAlphaNumeric = char.IsLetterOrDigit(rightStartingCharacter);
        if (leftEndsWithAlphaNumeric && rightStartsWithAlphaNumeric)
        {
            return 0;
        }

        bool leftEndsWithWhitespace = !leftEndsWithAlphaNumeric && char.IsWhiteSpace(leftEndingCharacter);
        bool rightStartsWithWhitespace = !rightStartsWithAlphaNumeric && char.IsWhiteSpace(rightStartingCharacter);
        if (!leftEndsWithWhitespace && !rightStartsWithWhitespace)
        {
            return 1;
        }

        bool leftEndsWithLineBreak = leftEndsWithWhitespace && char.IsControl(leftEndingCharacter);
        bool rightStartsWithLineBreak = rightStartsWithWhitespace && char.IsControl(rightStartingCharacter);
        bool leftEndsWithBlankLines = leftEndsWithLineBreak && BLANKLINEEND.IsMatch(left);
        bool rightStartsWithBlankLines = rightStartsWithLineBreak && BLANKLINESTART.IsMatch(right);

        if (leftEndsWithBlankLines || rightStartsWithBlankLines)
        {
            return 5;
        }
        else if (leftEndsWithLineBreak || rightStartsWithLineBreak)
        {
            return 4;
        }
        else if (!leftEndsWithAlphaNumeric && !leftEndsWithWhitespace && rightStartsWithWhitespace)
        {
            return 3;
        }

        return 2;
    }
}
