using System.Text;
using Mittons.DiffMatchPatch.Models;

namespace Mittons.DiffMatchPatch.Extensions;

public static class StringExtensions
{
    public static int IndexOf(this ReadOnlySpan<char> @this, ReadOnlySpan<char> value, int startIndex)
    {
        int newIndex = @this[startIndex..].IndexOf(value);

        return newIndex == -1 ? -1 : newIndex + startIndex;
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
        HashSet<string> set = [];
        List<string> lineArray = [];

        StringBuilder textResult = new();
        int lineStart = 0;
        int lineEnd = -1;
        string line;

        while (lineEnd < text1.Length - 1)
        {
            lineEnd = text1.IndexOf('\n', lineStart);
            if (lineEnd == -1)
            {
                lineEnd = text1.Length - 1;
            }
            line = text1[lineStart..(lineEnd + 1)];

            if (set.Add(line))
            {
                lineArray.Add(line);

                textResult.Append((char)(lineArray.Count - 1));
            }
            else
            {
                textResult.Append((char)lineArray.IndexOf(line));
            }

            lineStart = lineEnd + 1;
        }

        var text1Result = textResult.ToString();
        textResult.Clear();

        lineStart = 0;
        lineEnd = -1;

        while (lineEnd < text2.Length - 1)
        {
            lineEnd = text2.IndexOf('\n', lineStart);
            if (lineEnd == -1)
            {
                lineEnd = text2.Length - 1;
            }
            line = text2[lineStart..(lineEnd + 1)];

            if (set.Add(line))
            {
                lineArray.Add(line);

                textResult.Append((char)(lineArray.Count - 1));
            }
            else
            {
                textResult.Append((char)lineArray.IndexOf(line));
            }

            lineStart = lineEnd + 1;
        }

        return new(lineArray, text1Result, textResult.ToString());
    }

    // /**
    //     * Rehydrate the text in a diff from a string of line hashes to real lines
    //     * of text.
    //     * @param diffs List of Diff objects.
    //     * @param lineArray List of unique strings.
    //     */

    // public static IEnumerable<Diff> DecompressDiff(this IEnumerable<Diff> @this, IList<string> lookup)
    // {
    //     foreach (var diff in @this)
    //     {
    //         yield return diff with { Text = string.Join("", diff.Text.Select(x => lookup[x])) };
    //     }
    // }

    // private static (IEnumerable<Diff> OptimizedDiffs, string NextRetainedText) OptimizeSection(
    //     StringBuilder previousRetainedTextBuilder,
    //     StringBuilder deletedTextBuilder,
    //     StringBuilder insertedTextBuilder,
    //     StringBuilder nextRetainedTextBuilder
    // )
    // {
    //     var deletedText = deletedTextBuilder.ToString();
    //     var insertedText = insertedTextBuilder.ToString();

    //     deletedText.TryFindCommonSuffix(insertedText, out var commonSuffixSpan);
    //     var commonLength = commonSuffixSpan.Length;
    //     var nextRetainedText = commonLength == 0 ? nextRetainedTextBuilder.ToString() : $"{insertedText[^commonLength..]}{nextRetainedTextBuilder}";
    //     if (commonLength > 0)
    //     {
    //         insertedText = insertedText[..^commonLength];
    //         deletedText = deletedText[..^commonLength];
    //     }

    //     deletedText.TryFindCommonPrefix(insertedText, out var commonPrefixSpan);
    //     commonLength = commonPrefixSpan.Length;
    //     if (commonLength > 0)
    //     {
    //         previousRetainedTextBuilder.Append(insertedText[..commonLength]);

    //         insertedText = insertedText[commonLength..];
    //         deletedText = deletedText[commonLength..];
    //     }

    //     var previousRetainedText = previousRetainedTextBuilder.ToString();

    //     var hasSingleModifier = string.IsNullOrEmpty(deletedText) != string.IsNullOrEmpty(insertedText);
    //     var isBoundedByRetainedText = !string.IsNullOrEmpty(previousRetainedText) && !string.IsNullOrEmpty(nextRetainedText);
    //     var skipOptimization = !hasSingleModifier || !isBoundedByRetainedText;

    //     if (skipOptimization)
    //     {
    //         List<Diff> optimizedDiffs = [];

    //         if (previousRetainedText.Length > 0)
    //         {
    //             optimizedDiffs.Add(new Diff(Operation.EQUAL, previousRetainedText));
    //         }

    //         if (deletedText.Length > 0)
    //         {
    //             optimizedDiffs.Add(new Diff(Operation.DELETE, deletedText));
    //         }

    //         if (insertedText.Length > 0)
    //         {
    //             optimizedDiffs.Add(new Diff(Operation.INSERT, insertedText));
    //         }

    //         return (optimizedDiffs, nextRetainedText);
    //     }

    //     var modifiedOperation = string.IsNullOrEmpty(deletedText) ? Operation.INSERT : Operation.DELETE;
    //     var modifiedText = string.IsNullOrEmpty(deletedText) ? insertedText : deletedText;

    //     if (modifiedText.EndsWith(previousRetainedText, StringComparison.Ordinal))
    //     {
    //         return (
    //             [new Diff(modifiedOperation, $"{previousRetainedText}{modifiedText[..previousRetainedText.Length]}")],
    //             $"{previousRetainedText}{nextRetainedText}"
    //         );
    //     }

    //     if (modifiedText.StartsWith(nextRetainedText, StringComparison.Ordinal))
    //     {
    //         return (
    //             [
    //                 new Diff(Operation.EQUAL, $"{previousRetainedText}{nextRetainedText}"),
    //                 new Diff(modifiedOperation, $"{modifiedText[nextRetainedText.Length..]}{nextRetainedText}")
    //             ],
    //             string.Empty
    //         );
    //     }

    //     return (
    //         [new Diff(Operation.EQUAL, previousRetainedText), new Diff(modifiedOperation, modifiedText)],
    //         nextRetainedText
    //     );
    // }

    // public static IEnumerable<Diff> CleanupMerge(this IEnumerable<Diff> diffs)
    // {
    //     Operation[] modifierOperations = [Operation.INSERT, Operation.DELETE];

    //     StringBuilder deletedTextBuilder = new();
    //     StringBuilder insertedTextBuilder = new();
    //     StringBuilder previousRetainedTextBuilder = new();
    //     StringBuilder nextRetainedTextBuilder = new();

    //     IEnumerable<Diff> optimizedDiffs;
    //     string nextRetainedText;

    //     foreach (var diff in diffs)
    //     {
    //         if (string.IsNullOrEmpty(diff.Text))
    //         {
    //             continue;
    //         }

    //         if (modifierOperations.Contains(diff.Operation) && (nextRetainedTextBuilder.Length > 0))
    //         {
    //             (optimizedDiffs, nextRetainedText) = OptimizeSection(
    //                 previousRetainedTextBuilder,
    //                 deletedTextBuilder,
    //                 insertedTextBuilder,
    //                 nextRetainedTextBuilder
    //             );

    //             foreach (var optimizedDiff in optimizedDiffs)
    //             {
    //                 yield return optimizedDiff;
    //             }

    //             deletedTextBuilder.Clear();
    //             insertedTextBuilder.Clear();
    //             previousRetainedTextBuilder.Clear();
    //             nextRetainedTextBuilder.Clear();

    //             if (!string.IsNullOrEmpty(nextRetainedText))
    //             {
    //                 previousRetainedTextBuilder.Append(nextRetainedText);
    //             }
    //         }

    //         switch (diff.Operation)
    //         {
    //             case Operation.INSERT:
    //                 insertedTextBuilder.Append(diff.Text);

    //                 break;
    //             case Operation.DELETE:
    //                 deletedTextBuilder.Append(diff.Text);

    //                 break;
    //             case Operation.EQUAL:
    //                 if ((insertedTextBuilder.Length == 0) && (deletedTextBuilder.Length == 0))
    //                 {
    //                     previousRetainedTextBuilder.Append(diff.Text);
    //                 }
    //                 else
    //                 {
    //                     nextRetainedTextBuilder.Append(diff.Text);
    //                 }

    //                 break;
    //         }
    //     }

    //     (optimizedDiffs, nextRetainedText) = OptimizeSection(
    //         previousRetainedTextBuilder,
    //         deletedTextBuilder,
    //         insertedTextBuilder,
    //         nextRetainedTextBuilder
    //     );

    //     foreach (var optimizedDiff in optimizedDiffs)
    //     {
    //         yield return optimizedDiff;
    //     }

    //     if (!string.IsNullOrEmpty(nextRetainedText))
    //     {
    //         yield return new Diff(Operation.EQUAL, nextRetainedText);
    //     }
    // }

    // public static IEnumerable<Diff> CleanupSemanticLossless(this IEnumerable<Diff> diffs)
    // {
    //     List<Diff> previousDiffs = [];
    //     int commonOffset;

    //     foreach (var currentDiff in diffs)
    //     {
    //         switch (previousDiffs.Count)
    //         {
    //             case 0:
    //                 if (currentDiff.Operation == Operation.EQUAL)
    //                 {
    //                     previousDiffs.Add(currentDiff);
    //                 }
    //                 else
    //                 {
    //                     yield return currentDiff;
    //                 }

    //                 continue;
    //             case 1:
    //                 previousDiffs.Add(currentDiff);

    //                 continue;
    //             case 2:
    //                 if (currentDiff.Operation != Operation.EQUAL)
    //                 {
    //                     yield return previousDiffs[0];
    //                     yield return previousDiffs[1];
    //                     yield return currentDiff;

    //                     previousDiffs.Clear();

    //                     continue;
    //                 }

    //                 break;
    //         }

    //         string equality1 = previousDiffs[0].Text;
    //         string edit = previousDiffs[1].Text;
    //         string equality2 = currentDiff.Text;

    //         // First, shift the edit as far left as possible
    //         equality1.TryFindCommonSuffix(edit, out var commonSuffixSpan);
    //         commonOffset = commonSuffixSpan.Length;
    //         if (commonOffset > 0)
    //         {
    //             var commonString = edit[^commonOffset..];
    //             equality1 = equality1[..^commonOffset];
    //             edit = $"{commonString}{edit[..^commonOffset]}";
    //             equality2 = $"{commonString}{equality2}";
    //         }

    //         // Second, step character by character right, looking for the best fit
    //         string bestEquality1 = equality1;
    //         string bestEdit = edit;
    //         string bestEquality2 = equality2;
    //         var bestScore = ComputeSemanticComparisonScore(equality1, edit) + ComputeSemanticComparisonScore(edit, equality2);

    //         while (edit.Length != 0 && equality2.Length != 0 && edit[0] == equality2[0])
    //         {
    //             equality1 += edit[0];
    //             edit = $"{edit[1..]}{equality2[0]}";
    //             equality2 = equality2[1..];

    //             var score = ComputeSemanticComparisonScore(equality1, edit) + ComputeSemanticComparisonScore(edit, equality2);

    //             // The >= encourages trailing rather than leading whitespace on edits.
    //             if (score >= bestScore)
    //             {
    //                 bestScore = score;
    //                 bestEquality1 = equality1;
    //                 bestEdit = edit;
    //                 bestEquality2 = equality2;
    //             }
    //         }

    //         if (previousDiffs[0].Text == bestEquality1)
    //         {
    //             yield return previousDiffs[0];
    //             yield return previousDiffs[1];
    //             previousDiffs.Clear();
    //             previousDiffs.Add(currentDiff);

    //             continue;
    //         }

    //         if (bestEquality1.Length != 0)
    //         {
    //             yield return new Diff(Operation.EQUAL, bestEquality1);
    //         }

    //         yield return previousDiffs[1] with { Text = bestEdit };

    //         previousDiffs.Clear();
    //         if (bestEquality2.Length != 0)
    //         {
    //             previousDiffs.Add(new Diff(Operation.EQUAL, bestEquality2));
    //         }
    //     }

    //     foreach (var diff in previousDiffs)
    //     {
    //         yield return diff;
    //     }
    // }

    // /// <summary>
    // /// Given two strings, compute a score representing whether the internal
    // /// boundary falls on logical boundaries.
    // /// </summary>
    // /// <param name="left">The string whose end boundary is being evaluated.</param>
    // /// <param name="right">The string whose start boundary is being evaluated.</param>
    // /// <returns>The score representing the comparison result.</returns>
    // /// <remarks>
    // /// Scores range from 0 (worst) to 6 (best):
    // /// <list type="number">
    // /// <item>No match</item>
    // /// <item>Non-alphanumeric</item>
    // /// <item>Whitespace</item>
    // /// <item>End of sentence</item>
    // /// <item>Line break</item>
    // /// <item>Blank lines</item>
    // /// <item>Edge match, both strings are empty</item>
    // /// </list>
    // /// </remarks>
    // private static int ComputeSemanticComparisonScore(string left, string right)
    // {
    //     Regex BLANKLINEEND = new("\\n\\r?\\n\\Z");
    //     Regex BLANKLINESTART = new("\\A\\r?\\n\\r?\\n");

    //     if (left.Length == 0 || right.Length == 0)
    //     {
    //         return 6;
    //     }

    //     char leftEndingCharacter = left[^1];
    //     char rightStartingCharacter = right[0];

    //     bool leftEndsWithAlphaNumeric = char.IsLetterOrDigit(leftEndingCharacter);
    //     bool rightStartsWithAlphaNumeric = char.IsLetterOrDigit(rightStartingCharacter);
    //     if (leftEndsWithAlphaNumeric && rightStartsWithAlphaNumeric)
    //     {
    //         return 0;
    //     }

    //     bool leftEndsWithWhitespace = !leftEndsWithAlphaNumeric && char.IsWhiteSpace(leftEndingCharacter);
    //     bool rightStartsWithWhitespace = !rightStartsWithAlphaNumeric && char.IsWhiteSpace(rightStartingCharacter);
    //     if (!leftEndsWithWhitespace && !rightStartsWithWhitespace)
    //     {
    //         return 1;
    //     }

    //     bool leftEndsWithLineBreak = leftEndsWithWhitespace && char.IsControl(leftEndingCharacter);
    //     bool rightStartsWithLineBreak = rightStartsWithWhitespace && char.IsControl(rightStartingCharacter);
    //     bool leftEndsWithBlankLines = leftEndsWithLineBreak && BLANKLINEEND.IsMatch(left);
    //     bool rightStartsWithBlankLines = rightStartsWithLineBreak && BLANKLINESTART.IsMatch(right);

    //     if (leftEndsWithBlankLines || rightStartsWithBlankLines)
    //     {
    //         return 5;
    //     }
    //     else if (leftEndsWithLineBreak || rightStartsWithLineBreak)
    //     {
    //         return 4;
    //     }
    //     else if (!leftEndsWithAlphaNumeric && !leftEndsWithWhitespace && rightStartsWithWhitespace)
    //     {
    //         return 3;
    //     }

    //     return 2;
    // }

    // public static IEnumerable<Diff> CleanupSemantic(this IEnumerable<Diff> diffs)
    // {
    //     var diffsList = diffs.ToList();
    //     bool changes = false;
    //     // Stack of indices where equalities are found.
    //     Stack<int> equalities = [];
    //     // Always equal to equalities[equalitiesLength-1][1]
    //     string? lastEquality = null;
    //     int pointer = 0;  // Index of current position.
    //                       // Number of characters that changed prior to the equality.
    //     int length_insertions1 = 0;
    //     int length_deletions1 = 0;
    //     // Number of characters that changed after the equality.
    //     int length_insertions2 = 0;
    //     int length_deletions2 = 0;
    //     while (pointer < diffsList.Count)
    //     {
    //         if (diffsList[pointer].Operation == Operation.EQUAL)
    //         {  // Equality found.
    //             equalities.Push(pointer);
    //             length_insertions1 = length_insertions2;
    //             length_deletions1 = length_deletions2;
    //             length_insertions2 = 0;
    //             length_deletions2 = 0;
    //             lastEquality = diffsList[pointer].Text;
    //         }
    //         else
    //         {  // an insertion or deletion
    //             if (diffsList[pointer].Operation == Operation.INSERT)
    //             {
    //                 length_insertions2 += diffsList[pointer].Text.Length;
    //             }
    //             else
    //             {
    //                 length_deletions2 += diffsList[pointer].Text.Length;
    //             }
    //             // Eliminate an equality that is smaller or equal to the edits on both
    //             // sides of it.
    //             if (lastEquality != null && (lastEquality.Length
    //                 <= Math.Max(length_insertions1, length_deletions1))
    //                 && (lastEquality.Length
    //                     <= Math.Max(length_insertions2, length_deletions2)))
    //             {
    //                 // Duplicate record.
    //                 diffsList.Insert(equalities.Peek(),
    //                                 new Diff(Operation.DELETE, lastEquality));
    //                 // Change second copy to insert.
    //                 diffsList[equalities.Peek() + 1] = diffsList[equalities.Peek() + 1] with
    //                 {
    //                     Operation = Operation.INSERT
    //                 };
    //                 // Throw away the equality we just deleted.
    //                 equalities.Pop();
    //                 if (equalities.Count > 0)
    //                 {
    //                     equalities.Pop();
    //                 }
    //                 pointer = equalities.Count > 0 ? equalities.Peek() : -1;
    //                 length_insertions1 = 0;  // Reset the counters.
    //                 length_deletions1 = 0;
    //                 length_insertions2 = 0;
    //                 length_deletions2 = 0;
    //                 lastEquality = null;
    //                 changes = true;
    //             }
    //         }
    //         pointer++;
    //     }

    //     var normalizedDiffs = CleanupSemanticLossless(changes ? CleanupMerge(diffsList) : diffsList).ToList();

    //     // Find any overlaps between deletions and insertions.
    //     // e.g: <del>abcxxx</del><ins>xxxdef</ins>
    //     //   -> <del>abc</del>xxx<ins>def</ins>
    //     // e.g: <del>xxxabc</del><ins>defxxx</ins>
    //     //   -> <ins>def</ins>xxx<del>abc</del>
    //     // Only extract an overlap if it is as big as the edit ahead or behind it.
    //     pointer = 1;
    //     // Diff? previousDiff = null;

    //     while (pointer < normalizedDiffs.Count)
    //     {
    //         // var currentDiff = normalizedDiffs[pointer];

    //         // pointer++;

    //         // if (currentDiff.Operation == Operation.DELETE)
    //         // {
    //         //     previousDiff = currentDiff;

    //         //     continue;
    //         // }

    //         // if (previousDiff is null || currentDiff.Operation == Operation.EQUAL)
    //         // {
    //         //     if (previousDiff is not null)
    //         //     {
    //         //         yield return previousDiff;

    //         //         previousDiff = null;
    //         //     }

    //         //     yield return currentDiff;

    //         //     continue;
    //         // }

    //         // string deletion = previousDiff.Text;
    //         // string insertion = normalizedDiffs[pointer].Text;

    //         // var minimumLength = Math.Min(deletion.Length, insertion.Length) / 2.0;

    //         // var deletionEndsWithInsertion = deletion.TryFindCommonOverlap(insertion, out var commonOverlapSpan1) && commonOverlapSpan1.Length >= minimumLength;
    //         // var insertionEndsWithDeletion = insertion.TryFindCommonOverlap(deletion, out var commonOverlapSpan2) && commonOverlapSpan2.Length >= minimumLength;

    //         // if (!deletionEndsWithInsertion && !insertionEndsWithDeletion)
    //         // {
    //         //     // No overlap found.
    //         //     yield return previousDiff;
    //         //     yield return currentDiff;

    //         //     previousDiff = null;

    //         //     continue;
    //         // }

    //         // if (deletionEndsWithInsertion &&commonOverlapSpan1.Length >= commonOverlapSpan2.Length)
    //         // {
    //         //     // Overlap found.
    //         //     // Insert an equality and trim the surrounding edits.
    //         //     normalizedDiffs.Insert(pointer, new Diff(Operation.EQUAL,
    //         //         insertion.Substring(0, commonOverlapSpan1.Length)));
    //         //     normalizedDiffs[pointer - 1] = normalizedDiffs[pointer - 1] with
    //         //     {
    //         //         Text = deletion[..^commonOverlapSpan1.Length]
    //         //     };
    //         //     normalizedDiffs[pointer + 1] = normalizedDiffs[pointer + 1] with
    //         //     {
    //         //         Text = insertion[commonOverlapSpan1.Length..]
    //         //     };
    //         //     pointer++;
    //         // }
    //         // else if (insertionEndsWithDeletion)
    //         // {
    //         //     if (commonOverlapSpan2.Length >= minimumLength)
    //         //     {
    //         //         // Reverse overlap found.
    //         //         // Insert an equality and swap and trim the surrounding edits.
    //         //         normalizedDiffs.Insert(pointer, new Diff(Operation.EQUAL, deletion[..commonOverlapSpan2.Length]));
    //         //         normalizedDiffs[pointer - 1] = new(Operation.INSERT, insertion[..^commonOverlapSpan2.Length]);
    //         //         normalizedDiffs[pointer + 1] = new(Operation.DELETE, deletion[commonOverlapSpan2.Length..]);
    //         //         pointer++;
    //         //     }
    //         // }

    //         if (normalizedDiffs[pointer - 1].Operation == Operation.DELETE &&
    //                 normalizedDiffs[pointer].Operation == Operation.INSERT)
    //         {
    //             string deletion = normalizedDiffs[pointer - 1].Text;
    //             string insertion = normalizedDiffs[pointer].Text;
    //             var deletionEndsWithInsertion = deletion.TryFindCommonOverlap(insertion, out var commonOverlapSpan1);
    //             var insertionEndsWithDeletion = insertion.TryFindCommonOverlap(deletion, out var commonOverlapSpan2);

    //             if (deletionEndsWithInsertion && (!insertionEndsWithDeletion || commonOverlapSpan1.Length >= commonOverlapSpan2.Length))
    //             {
    //                 if (commonOverlapSpan1.Length >= deletion.Length / 2.0 ||
    //                     commonOverlapSpan1.Length >= insertion.Length / 2.0)
    //                 {
    //                     // Overlap found.
    //                     // Insert an equality and trim the surrounding edits.
    //                     normalizedDiffs.Insert(pointer, new Diff(Operation.EQUAL,
    //                         insertion.Substring(0, commonOverlapSpan1.Length)));
    //                     normalizedDiffs[pointer - 1] = normalizedDiffs[pointer - 1] with
    //                     {
    //                         Text = deletion[..^commonOverlapSpan1.Length]
    //                     };
    //                     normalizedDiffs[pointer + 1] = normalizedDiffs[pointer + 1] with
    //                     {
    //                         Text = insertion[commonOverlapSpan1.Length..]
    //                     };
    //                     pointer++;
    //                 }
    //             }
    //             else if (insertionEndsWithDeletion)
    //             {
    //                 if (commonOverlapSpan2.Length >= deletion.Length / 2.0 ||
    //                     commonOverlapSpan2.Length >= insertion.Length / 2.0)
    //                 {
    //                     // Reverse overlap found.
    //                     // Insert an equality and swap and trim the surrounding edits.
    //                     normalizedDiffs.Insert(pointer, new Diff(Operation.EQUAL, deletion[..commonOverlapSpan2.Length]));
    //                     normalizedDiffs[pointer - 1] = new(Operation.INSERT, insertion[..^commonOverlapSpan2.Length]);
    //                     normalizedDiffs[pointer + 1] = new(Operation.DELETE, deletion[commonOverlapSpan2.Length..]);
    //                     pointer++;
    //                 }
    //             }
    //             pointer++;
    //         }
    //         pointer++;
    //     }

    //     return normalizedDiffs;
    // }

    // /**
    //     * Reduce the number of edits by eliminating operationally trivial
    //     * equalities.
    //     * @param diffs List of Diff objects.
    //     */
    // public static IEnumerable<Diff> CleanupEfficiency(this IEnumerable<Diff> diffs, short editCost)
    // {
    //     var diffsList = diffs.ToList();

    //     bool changes = false;
    //     // Stack of indices where equalities are found.
    //     Stack<int> equalities = new Stack<int>();
    //     // Always equal to equalities[equalitiesLength-1][1]
    //     string lastEquality = string.Empty;
    //     int pointer = 0;  // Index of current position.
    //                       // Is there an insertion operation before the last equality.
    //     bool pre_ins = false;
    //     // Is there a deletion operation before the last equality.
    //     bool pre_del = false;
    //     // Is there an insertion operation after the last equality.
    //     bool post_ins = false;
    //     // Is there a deletion operation after the last equality.
    //     bool post_del = false;
    //     while (pointer < diffsList.Count)
    //     {
    //         if (diffsList[pointer].Operation == Operation.EQUAL)
    //         {  // Equality found.
    //             if (diffsList[pointer].Text.Length < editCost
    //                 && (post_ins || post_del))
    //             {
    //                 // Candidate found.
    //                 equalities.Push(pointer);
    //                 pre_ins = post_ins;
    //                 pre_del = post_del;
    //                 lastEquality = diffsList[pointer].Text;
    //             }
    //             else
    //             {
    //                 // Not a candidate, and can never become one.
    //                 equalities.Clear();
    //                 lastEquality = string.Empty;
    //             }
    //             post_ins = post_del = false;
    //         }
    //         else
    //         {  // An insertion or deletion.
    //             if (diffsList[pointer].Operation == Operation.DELETE)
    //             {
    //                 post_del = true;
    //             }
    //             else
    //             {
    //                 post_ins = true;
    //             }
    //             /*
    //                 * Five types to be split:
    //                 * <ins>A</ins><del>B</del>XY<ins>C</ins><del>D</del>
    //                 * <ins>A</ins>X<ins>C</ins><del>D</del>
    //                 * <ins>A</ins><del>B</del>X<ins>C</ins>
    //                 * <ins>A</del>X<ins>C</ins><del>D</del>
    //                 * <ins>A</ins><del>B</del>X<del>C</del>
    //                 */
    //             if ((lastEquality.Length != 0)
    //                 && ((pre_ins && pre_del && post_ins && post_del)
    //                 || ((lastEquality.Length < editCost / 2)
    //                 && ((pre_ins ? 1 : 0) + (pre_del ? 1 : 0) + (post_ins ? 1 : 0)
    //                 + (post_del ? 1 : 0)) == 3)))
    //             {
    //                 // Duplicate record.
    //                 diffsList.Insert(equalities.Peek(),
    //                                 new Diff(Operation.DELETE, lastEquality));
    //                 // Change second copy to insert.
    //                 diffsList[equalities.Peek() + 1] = diffsList[equalities.Peek() + 1] with { Operation = Operation.INSERT };
    //                 equalities.Pop();  // Throw away the equality we just deleted.
    //                 lastEquality = string.Empty;
    //                 if (pre_ins && pre_del)
    //                 {
    //                     // No changes made which could affect previous entry, keep going.
    //                     post_ins = post_del = true;
    //                     equalities.Clear();
    //                 }
    //                 else
    //                 {
    //                     if (equalities.Count > 0)
    //                     {
    //                         equalities.Pop();
    //                     }

    //                     pointer = equalities.Count > 0 ? equalities.Peek() : -1;
    //                     post_ins = post_del = false;
    //                 }
    //                 changes = true;
    //             }
    //         }
    //         pointer++;
    //     }

    //     return changes ? CleanupMerge(diffsList) : diffsList;
    // }

    // /**
    //     * Convert a Diff list into a pretty HTML report.
    //     * @param diffs List of Diff objects.
    //     * @return HTML representation.
    //     */
    // public static string CreateHtmlReport(this IEnumerable<Diff> diffs)
    // {
    //     StringBuilder html = new();

    //     foreach (Diff aDiff in diffs)
    //     {
    //         string text = aDiff.Text.Replace("&", "&amp;").Replace("<", "&lt;")
    //             .Replace(">", "&gt;").Replace("\n", "&para;<br>");
    //         switch (aDiff.Operation)
    //         {
    //             case Operation.INSERT:
    //                 html.Append($"""<ins style="background:#e6ffe6;">{text}</ins>""");
    //                 break;
    //             case Operation.DELETE:
    //                 html.Append($"""<del style="background:#ffe6e6;">{text}</del>""");
    //                 break;
    //             case Operation.EQUAL:
    //                 html.Append($"<span>{text}</span>");
    //                 break;
    //         }
    //     }

    //     return html.ToString();
    // }

    // /**
    //     * Compute and return the source text (all equalities and deletions).
    //     * @param diffs List of Diff objects.
    //     * @return Source text.
    //     */
    // public static string ComposeOriginalText(this IEnumerable<Diff> diffs)
    // {
    //     StringBuilder text = new();

    //     foreach (Diff aDiff in diffs)
    //     {
    //         if (aDiff.Operation != Operation.INSERT)
    //         {
    //             text.Append(aDiff.Text);
    //         }
    //     }

    //     return text.ToString();
    // }

    // /**
    //     * Compute and return the destination text (all equalities and insertions).
    //     * @param diffs List of Diff objects.
    //     * @return Destination text.
    //     */
    // public static string ComposeFinalText(this IEnumerable<Diff> diffs)
    // {
    //     StringBuilder text = new();

    //     foreach (Diff aDiff in diffs)
    //     {
    //         if (aDiff.Operation != Operation.DELETE)
    //         {
    //             text.Append(aDiff.Text);
    //         }
    //     }

    //     return text.ToString();
    // }

    // /**
    //     * Encodes a string with URI-style % escaping.
    //     * Compatible with JavaScript's encodeURI function.
    //     *
    //     * @param str The string to encode.
    //     * @return The encoded string.
    //     */
    // public static string EncodeUri(this string str)
    // {
    //     // C# is overzealous in the replacements.  Walk back on a few.
    //     return new StringBuilder(HttpUtility.UrlEncode(str))
    //         .Replace('+', ' ').Replace("%20", " ").Replace("%21", "!")
    //         .Replace("%2a", "*").Replace("%27", "'").Replace("%28", "(")
    //         .Replace("%29", ")").Replace("%3b", ";").Replace("%2f", "/")
    //         .Replace("%3f", "?").Replace("%3a", ":").Replace("%40", "@")
    //         .Replace("%26", "&").Replace("%3d", "=").Replace("%2b", "+")
    //         .Replace("%24", "$").Replace("%2c", ",").Replace("%23", "#")
    //         .Replace("%7e", "~")
    //         .ToString();
    // }

    // /**
    //     * Crush the diff into an encoded string which describes the operations
    //     * required to transform text1 into text2.
    //     * E.g. =3\t-2\t+ing  -> Keep 3 chars, delete 2 chars, insert 'ing'.
    //     * Operations are tab-separated.  Inserted text is escaped using %xx
    //     * notation.
    //     * @param diffs Array of Diff objects.
    //     * @return Delta text.
    //     */
    // public static string ToDeltaEncodedString(this IEnumerable<Diff> diffs)
    // {
    //     StringBuilder text = new();

    //     foreach (Diff aDiff in diffs)
    //     {
    //         if (aDiff.Text.Length == 0)
    //         {
    //             continue;
    //         }

    //         switch (aDiff.Operation)
    //         {
    //             case Operation.INSERT:
    //                 text.Append($"+{aDiff.Text.EncodeUri()}\t");
    //                 break;
    //             case Operation.DELETE:
    //                 text.Append($"-{aDiff.Text.Length}\t");
    //                 break;
    //             case Operation.EQUAL:
    //                 text.Append($"={aDiff.Text.Length}\t");
    //                 break;
    //         }
    //     }

    //     return text.Length == 0 ? string.Empty : text.ToString()[..^1];
    // }

    // /**
    //  * Given the original text1, and an encoded string which describes the
    //  * operations required to transform text1 into text2, compute the full diff.
    //  * @param text1 Source string for the diff.
    //  * @param delta Delta text.
    //  * @return Array of Diff objects or null if invalid.
    //  * @throws ArgumentException If invalid input.
    //  */
    // public static IEnumerable<Diff> ToDiff(this string sourceText, string delta)
    // {
    //     int pointer = 0;
    //     string text;

    //     foreach (string token in delta.Split(["\t"], StringSplitOptions.None))
    //     {
    //         if (token.Length == 0)
    //         {
    //             continue;
    //         }

    //         // Each token begins with a one character parameter which specifies the
    //         // operation of this token (delete, insert, equality).
    //         var operation = token[0].ToOperation();
    //         var value = token[1..];

    //         switch (operation)
    //         {
    //             case Operation.INSERT:
    //                 text = HttpUtility.UrlDecode(value.Replace("+", "%2b"));

    //                 break;
    //             case Operation.DELETE:
    //             case Operation.EQUAL:
    //                 if (!int.TryParse(value, out var n))
    //                 {
    //                     throw new ArgumentException($"Invalid number in diff_fromDelta: {value}");
    //                 }

    //                 if (n < 0)
    //                 {
    //                     throw new ArgumentException($"Negative number in diff_fromDelta: {value}");
    //                 }

    //                 try
    //                 {
    //                     text = sourceText.Substring(pointer, n);
    //                     pointer += n;
    //                 }
    //                 catch (ArgumentOutOfRangeException e)
    //                 {
    //                     throw new ArgumentException($"Delta length ({pointer}) larger than source text length ({sourceText.Length}).", e);
    //                 }

    //                 break;
    //             default:
    //                 throw new ArgumentException($"Invalid diff operation in diff_fromDelta: {token[0]}");
    //         }

    //         yield return new Diff(operation, text);
    //     }

    //     if (pointer != sourceText.Length)
    //     {
    //         throw new ArgumentException($"Delta length ({pointer}) smaller than source text length ({sourceText.Length}).");
    //     }
    // }

    // /**
    //     * loc is a location in text1, compute and return the equivalent location in
    //     * text2.
    //     * e.g. "The cat" vs "The big cat", 1->1, 5->8
    //     * @param diffs List of Diff objects.
    //     * @param loc Location within text1.
    //     * @return Location within text2.
    //     */
    // public static int ConvertLocationInOriginalTextToLocationInFinalText(this IEnumerable<Diff> diffs, int loc)
    // {
    //     int currentPositionInOriginalText = 0;
    //     int previousPositionInOriginalText = 0;
    //     int currentPositionInFinalText = 0;
    //     int previousPositionInFinalText = 0;

    //     foreach (Diff aDiff in diffs)
    //     {
    //         if (aDiff.Operation != Operation.INSERT)
    //         {
    //             // Equality or deletion.
    //             currentPositionInOriginalText += aDiff.Text.Length;
    //         }
    //         if (aDiff.Operation != Operation.DELETE)
    //         {
    //             // Equality or insertion.
    //             currentPositionInFinalText += aDiff.Text.Length;
    //         }
    //         if (currentPositionInOriginalText > loc)
    //         {
    //             // The location was deleted
    //             if (aDiff.Operation == Operation.DELETE)
    //             {
    //                 return previousPositionInFinalText;
    //             }

    //             break;
    //         }

    //         previousPositionInOriginalText = currentPositionInOriginalText;
    //         previousPositionInFinalText = currentPositionInFinalText;
    //     }

    //     // Add the remaining character length.
    //     return previousPositionInFinalText + (loc - previousPositionInOriginalText);
    // }

    // /**
    //     * Compute the Levenshtein distance; the number of inserted, deleted or
    //     * substituted characters.
    //     * @param diffs List of Diff objects.
    //     * @return Number of changes.
    //     */
    // public static int CalculateLevenshteinDistance(this IEnumerable<Diff> diffs)
    // {
    //     int levenshtein = 0;
    //     int insertions = 0;
    //     int deletions = 0;

    //     foreach (Diff aDiff in diffs)
    //     {
    //         switch (aDiff.Operation)
    //         {
    //             case Operation.INSERT:
    //                 insertions += aDiff.Text.Length;
    //                 break;
    //             case Operation.DELETE:
    //                 deletions += aDiff.Text.Length;
    //                 break;
    //             case Operation.EQUAL:
    //                 // A deletion and an insertion is one substitution.
    //                 levenshtein += Math.Max(insertions, deletions);
    //                 insertions = 0;
    //                 deletions = 0;
    //                 break;
    //         }
    //     }

    //     return levenshtein + Math.Max(insertions, deletions);
    // }

    // /**
    //     * Find the 'middle snake' of a diff, split the problem in two
    //     * and return the recursively constructed diff.
    //     * See Myers 1986 paper: An O(ND) Difference Algorithm and Its Variations.
    //     * @param text1 Old string to be diffed.
    //     * @param text2 New string to be diffed.
    //     * @param deadline Time at which to bail if not yet complete.
    //     * @return List of Diff objects.
    //     */
    // public static IEnumerable<Diff> BisectTexts(this string text1, string text2, CancellationToken cancellationToken)
    // {
    //     // Cache the text lengths to prevent multiple calls.
    //     int text1_length = text1.Length;
    //     int text2_length = text2.Length;
    //     int max_d = (text1_length + text2_length + 1) / 2;
    //     int v_offset = max_d;
    //     int v_length = 2 * max_d;

    //     int[] v1 = new int[v_length];
    //     int[] v2 = new int[v_length];
    //     for (int x = 0; x < v_length; x++)
    //     {
    //         v1[x] = -1;
    //         v2[x] = -1;
    //     }

    //     v1[v_offset + 1] = 0;
    //     v2[v_offset + 1] = 0;

    //     int delta = text1_length - text2_length;

    //     // If the total number of characters is odd, then the front path will
    //     // collide with the reverse path.
    //     bool front = delta % 2 != 0;

    //     // Offsets for start and end of k loop.
    //     // Prevents mapping of space beyond the grid.
    //     int k1start = 0;
    //     int k1end = 0;
    //     int k2start = 0;
    //     int k2end = 0;
    //     for (int d = 0; d < max_d; d++)
    //     {
    //         // Bail out if deadline is reached.
    //         if (cancellationToken.IsCancellationRequested)
    //         {
    //             break;
    //         }

    //         // Walk the front path one step.
    //         for (int k1 = -d + k1start; k1 <= d - k1end; k1 += 2)
    //         {
    //             int k1_offset = v_offset + k1;
    //             int x1;
    //             if (k1 == -d || k1 != d && v1[k1_offset - 1] < v1[k1_offset + 1])
    //             {
    //                 x1 = v1[k1_offset + 1];
    //             }
    //             else
    //             {
    //                 x1 = v1[k1_offset - 1] + 1;
    //             }
    //             int y1 = x1 - k1;
    //             while (x1 < text1_length && y1 < text2_length
    //                     && text1[x1] == text2[y1])
    //             {
    //                 x1++;
    //                 y1++;
    //             }
    //             v1[k1_offset] = x1;
    //             if (x1 > text1_length)
    //             {
    //                 // Ran off the right of the graph.
    //                 k1end += 2;
    //             }
    //             else if (y1 > text2_length)
    //             {
    //                 // Ran off the bottom of the graph.
    //                 k1start += 2;
    //             }
    //             else if (front)
    //             {
    //                 int k2_offset = v_offset + delta - k1;
    //                 if (k2_offset >= 0 && k2_offset < v_length && v2[k2_offset] != -1)
    //                 {
    //                     // Mirror x2 onto top-left coordinate system.
    //                     int x2 = text1_length - v2[k2_offset];
    //                     if (x1 >= x2)
    //                     {
    //                         // Overlap detected.
    //                         return SplitTexts(text1, text2, x1, y1, cancellationToken);
    //                     }
    //                 }
    //             }
    //         }

    //         // Walk the reverse path one step.
    //         for (int k2 = -d + k2start; k2 <= d - k2end; k2 += 2)
    //         {
    //             int k2_offset = v_offset + k2;
    //             int x2;
    //             if (k2 == -d || k2 != d && v2[k2_offset - 1] < v2[k2_offset + 1])
    //             {
    //                 x2 = v2[k2_offset + 1];
    //             }
    //             else
    //             {
    //                 x2 = v2[k2_offset - 1] + 1;
    //             }
    //             int y2 = x2 - k2;
    //             while (x2 < text1_length && y2 < text2_length
    //                 && text1[text1_length - x2 - 1]
    //                 == text2[text2_length - y2 - 1])
    //             {
    //                 x2++;
    //                 y2++;
    //             }
    //             v2[k2_offset] = x2;
    //             if (x2 > text1_length)
    //             {
    //                 // Ran off the left of the graph.
    //                 k2end += 2;
    //             }
    //             else if (y2 > text2_length)
    //             {
    //                 // Ran off the top of the graph.
    //                 k2start += 2;
    //             }
    //             else if (!front)
    //             {
    //                 int k1_offset = v_offset + delta - k2;
    //                 if (k1_offset >= 0 && k1_offset < v_length && v1[k1_offset] != -1)
    //                 {
    //                     int x1 = v1[k1_offset];
    //                     int y1 = v_offset + x1 - k1_offset;
    //                     // Mirror x2 onto top-left coordinate system.
    //                     x2 = text1_length - v2[k2_offset];
    //                     if (x1 >= x2)
    //                     {
    //                         // Overlap detected.
    //                         return SplitTexts(text1, text2, x1, y1, cancellationToken);
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     // Diff took too long and hit the deadline or
    //     // number of diffs equals number of characters, no commonality at all.
    //     return [
    //         new Diff(Operation.DELETE, text1),
    //         new Diff(Operation.INSERT, text2)
    //     ];
    // }

    // /**
    //     * Given the location of the 'middle snake', split the diff in two parts
    //     * and recurse.
    //     * @param text1 Old string to be diffed.
    //     * @param text2 New string to be diffed.
    //     * @param x Index of split point in text1.
    //     * @param y Index of split point in text2.
    //     * @param deadline Time at which to bail if not yet complete.
    //     * @return LinkedList of Diff objects.
    //     */
    // public static List<Diff> SplitTexts(string text1, string text2, int x, int y, CancellationToken cancellationToken)
    // {
    //     string text1a = text1[..x];
    //     string text2a = text2[..y];
    //     string text1b = text1[x..];
    //     string text2b = text2[y..];

    //     // Compute both diffs serially.
    //     List<Diff> diffs = diff_main(text1a, text2a, false, cancellationToken).ToList();
    //     List<Diff> diffsb = diff_main(text1b, text2b, false, cancellationToken).ToList();

    //     diffs.AddRange(diffsb);
    //     return diffs;
    // }

    // // /**
    // //     * Find the differences between two texts.
    // //     * @param text1 Old string to be diffed.
    // //     * @param text2 New string to be diffed.
    // //     * @param checklines Speedup flag.  If false, then don't run a
    // //     *     line-level diff first to identify the changed areas.
    // //     *     If true, then run a faster slightly less optimal diff.
    // //     * @return List of Diff objects.
    // //     */
    // // public static IEnumerable<Diff> diff_main(string text1, string text2, bool checklines = true)
    // // {
    // //     if (!DiffTimeout.HasValue)
    // //     {
    // //         return diff_main(text1, text2, checklines, default);
    // //     }

    // //     using var cancellationTokenSource = new CancellationTokenSource(DiffTimeout.Value);
    // //     return diff_main(text1, text2, checklines, cancellationTokenSource.Token);
    // // }

    // /**
    //     * Find the differences between two texts.  Simplifies the problem by
    //     * stripping any common prefix or suffix off the texts before diffing.
    //     * @param text1 Old string to be diffed.
    //     * @param text2 New string to be diffed.
    //     * @param checklines Speedup flag.  If false, then don't run a
    //     *     line-level diff first to identify the changed areas.
    //     *     If true, then run a faster slightly less optimal diff.
    //     * @param deadline Time when the diff should be complete by.  Used
    //     *     internally for recursive calls.  Users should set DiffTimeout
    //     *     instead.
    //     * @return List of Diff objects.
    //     */
    // public static IEnumerable<Diff> diff_main(string text1, string text2, bool checklines, CancellationToken cancellationToken = default)
    // {
    //     // Check for null inputs not needed since null can't be passed in C#.

    //     // Check for equality (speedup).
    //     if (string.IsNullOrEmpty(text1) && string.IsNullOrEmpty(text2))
    //     {
    //         yield break;
    //     }

    //     if (text1 == text2)
    //     {
    //         yield return new Diff(Operation.EQUAL, text1);
    //     }

    //     var diffs = SplitPrefixAndSuffix(text1, text2, checklines, cancellationToken).CleanupMerge();
    //     foreach (var diff in diffs)
    //     {
    //         yield return diff;
    //     }
    // }

    // public static IEnumerable<Diff> SplitPrefixAndSuffix(
    //     string text1,
    //     string text2,
    //     bool checklines,
    //     CancellationToken cancellationToken)
    // {
    //     // // Trim off common prefix (speedup).
    //     // int commonlength = text1.CommonPrefixLength(text2);
    //     // string commonprefix = text1[..commonlength];
    //     // text1 = text1[commonlength..];
    //     // text2 = text2[commonlength..];

    //     // // Trim off common suffix (speedup).
    //     // commonlength = text1.CommonSuffixLength(text2);
    //     // string commonsuffix = text1[^commonlength..];
    //     // text1 = text1[..^commonlength];
    //     // text2 = text2[..^commonlength];

    //     // // Restore the prefix and suffix.
    //     // if (commonprefix.Length != 0)
    //     // {
    //     //     yield return new Diff(Operation.EQUAL, commonprefix);
    //     // }
    //     // foreach (var diff in diff_compute(text1, text2, checklines, cancellationToken))
    //     // {
    //     //     yield return diff;
    //     // }
    //     // if (commonsuffix.Length != 0)
    //     // {
    //     //     yield return new Diff(Operation.EQUAL, commonsuffix);
    //     // }

    //     yield break;
    // }

    // /**
    //     * Find the differences between two texts.  Assumes that the texts do not
    //     * have any common prefix or suffix.
    //     * @param text1 Old string to be diffed.
    //     * @param text2 New string to be diffed.
    //     * @param checklines Speedup flag.  If false, then don't run a
    //     *     line-level diff first to identify the changed areas.
    //     *     If true, then run a faster slightly less optimal diff.
    //     * @param deadline Time when the diff should be complete by.
    //     * @return List of Diff objects.
    //     */
    // public static IEnumerable<Diff> diff_compute(string text1, string text2,
    //                                 bool checklines, CancellationToken cancellationToken)
    // {
    //     if (text1.Length == 0)
    //     {
    //         // Just add some text (speedup).
    //         yield return new Diff(Operation.INSERT, text2);
    //     }
    //     else if (text2.Length == 0)
    //     {
    //         // Just delete some text (speedup).
    //         yield return new Diff(Operation.DELETE, text1);
    //     }

    //     string longtext = text1.Length > text2.Length ? text1 : text2;
    //     string shorttext = text1.Length > text2.Length ? text2 : text1;
    //     int i = longtext.IndexOf(shorttext, StringComparison.Ordinal);
    //     if (i != -1)
    //     {
    //         // Shorter text is inside the longer text (speedup).
    //         Operation op = (text1.Length > text2.Length) ?
    //             Operation.DELETE : Operation.INSERT;
    //         yield return new Diff(op, longtext.Substring(0, i));
    //         yield return new Diff(Operation.EQUAL, shorttext);
    //         yield return new Diff(op, longtext.Substring(i + shorttext.Length));
    //     }

    //     if (shorttext.Length == 1)
    //     {
    //         // Single character string.
    //         // After the previous speedup, the character can't be an equality.
    //         yield return new Diff(Operation.DELETE, text1);
    //         yield return new Diff(Operation.INSERT, text2);
    //     }

    //     // Check to see if the problem can be split in two.
    //     var wasCommonHalfMiddleFound = text1.TryFindCommonHalfMiddle(
    //         text2,
    //         out var leftPrefixSpan,
    //         out var leftSuffixSpan,
    //         out var rightPrefixSpan,
    //         out var rightSuffixSpan,
    //         out var commonMiddleSpan
    //     );

    //     if (wasCommonHalfMiddleFound)
    //     {
    //         // A half-match was found, sort out the return data.
    //         var leftPrefix = leftPrefixSpan.ToString();
    //         var leftSuffix = leftSuffixSpan.ToString();
    //         var rightPrefix = rightPrefixSpan.ToString();
    //         var rightSuffix = rightSuffixSpan.ToString();
    //         var commonMiddle = commonMiddleSpan.ToString();
    //         // Send both pairs off for separate processing.
    //         foreach (var diff in diff_main(leftPrefix, rightPrefix, checklines, cancellationToken))
    //         {
    //             yield return diff;
    //         }
    //         yield return new Diff(Operation.EQUAL, commonMiddle);
    //         foreach (var diff in diff_main(leftSuffix, rightSuffix, checklines, cancellationToken))
    //         {
    //             yield return diff;
    //         }
    //     }

    //     if (checklines && text1.Length > 100 && text2.Length > 100)
    //     {
    //         foreach (var diff in diff_lineMode(text1, text2, cancellationToken))
    //         {
    //             yield return diff;
    //         }
    //     }

    //     foreach (var diff in BisectTexts(text1, text2, cancellationToken))
    //     {
    //         yield return diff;
    //     }
    // }

    // /**
    //     * Do a quick line-level diff on both strings, then rediff the parts for
    //     * greater accuracy.
    //     * This speedup can produce non-minimal diffs.
    //     * @param text1 Old string to be diffed.
    //     * @param text2 New string to be diffed.
    //     * @param deadline Time when the diff should be complete by.
    //     * @return List of Diff objects.
    //     */
    // private static List<Diff> diff_lineMode(string text1, string text2,
    //                                     CancellationToken cancellationToken)
    // {
    //     // Scan the text on a line-by-line basis first.
    //     HashedTexts hashes = text1.CompressTexts(text2);

    //     text1 = hashes.Text1Hashed;
    //     text2 = hashes.Text2Hashed;
    //     List<string> linearray = hashes.HashLookup;

    //     List<Diff> diffs = [.. CleanupSemantic(diff_main(text1, text2, false, cancellationToken).DecompressDiff(linearray))];

    //     // Rediff any replacement blocks, this time character-by-character.
    //     // Add a dummy entry at the end.
    //     diffs.Add(new Diff(Operation.EQUAL, string.Empty));
    //     int pointer = 0;
    //     int count_delete = 0;
    //     int count_insert = 0;
    //     string text_delete = string.Empty;
    //     string text_insert = string.Empty;
    //     while (pointer < diffs.Count)
    //     {
    //         switch (diffs[pointer].Operation)
    //         {
    //             case Operation.INSERT:
    //                 count_insert++;
    //                 text_insert += diffs[pointer].Text;
    //                 break;
    //             case Operation.DELETE:
    //                 count_delete++;
    //                 text_delete += diffs[pointer].Text;
    //                 break;
    //             case Operation.EQUAL:
    //                 // Upon reaching an equality, check for prior redundancies.
    //                 if (count_delete >= 1 && count_insert >= 1)
    //                 {
    //                     // Delete the offending records and add the merged ones.
    //                     diffs.RemoveRange(pointer - count_delete - count_insert,
    //                         count_delete + count_insert);
    //                     pointer = pointer - count_delete - count_insert;
    //                     List<Diff> subDiff =
    //                         diff_main(text_delete, text_insert, false, cancellationToken).ToList();
    //                     diffs.InsertRange(pointer, subDiff);
    //                     pointer = pointer + subDiff.Count;
    //                 }
    //                 count_insert = 0;
    //                 count_delete = 0;
    //                 text_delete = string.Empty;
    //                 text_insert = string.Empty;
    //                 break;
    //         }
    //         pointer++;
    //     }
    //     diffs.RemoveAt(diffs.Count - 1);  // Remove the dummy entry at the end.

    //     return diffs;
    // }
}
