// using Mittons.DiffMatchPatch.Models;
// using Mittons.DiffMatchPatch.Types;

// namespace Mittons.DiffMatchPatch.Builders;

// public class BaseDiffBuilder(string originalText, string finalText)
// {
//     private readonly string _originalText = originalText;
//     private readonly string _finalText = finalText;

//     public IEnumerable<Diff> Build()
//     {
//         if (_originalText.Length == 0)
//         {
//             // Just add some text (speedup).
//             yield return new Diff(Operation.INSERT, _finalText);
//             yield break;
//         }

//         if (_finalText.Length == 0)
//         {
//             // Just delete some text (speedup).
//             yield return new Diff(Operation.DELETE, _originalText);

//             yield break;
//         }

//         var longerText = _originalText.Length > _finalText.Length ? _originalText : _finalText;
//         var shorterText = _originalText.Length > _finalText.Length ? _finalText : _originalText;

//         var commonStartIndex = longerText.IndexOf(shorterText, StringComparison.Ordinal);
//         if (commonStartIndex != -1)
//         {
//             Operation op = (_originalText.Length > _finalText.Length) ? Operation.DELETE : Operation.INSERT;

//             yield return new Diff(op, longerText[..commonStartIndex]);
//             yield return new Diff(Operation.EQUAL, shorterText);
//             yield return new Diff(op, longerText[(commonStartIndex + shorterText.Length)..]);

//             yield break;
//         }

//         // If the shorter text is a single character and wasn't found in the previous check, there's no shared equality.
//         if (shorterText.Length == 1)
//         {
//             yield return new Diff(Operation.DELETE, _originalText);
//             yield return new Diff(Operation.INSERT, _finalText);

//             yield break;
//         }

//         // Check to see if the problem can be split in two.
//         CommonMiddleDetails commonMiddleDetails = _originalText.CommonMiddle(_finalText, cancellationToken);
//         if (commonMiddleDetails.HasCommonMiddle)
//         {
//             // A half-match was found, sort out the return data.
//             var leftPrefix = commonMiddleDetails.LeftPrefix.ToString();
//             var leftSuffix = commonMiddleDetails.LeftSuffix.ToString();
//             var rightPrefix = commonMiddleDetails.RightPrefix.ToString();
//             var rightSuffix = commonMiddleDetails.RightSuffix.ToString();
//             var commonMiddle = commonMiddleDetails.CommonMiddle.ToString();
//             // Send both pairs off for separate processing.
//             foreach (var diff in diff_main(leftPrefix, rightPrefix, checklines, cancellationToken))
//             {
//                 yield return diff;
//             }
//             yield return new Diff(Operation.EQUAL, commonMiddle);
//             foreach (var diff in diff_main(leftSuffix, rightSuffix, checklines, cancellationToken))
//             {
//                 yield return diff;
//             }
//         }

//         if (checklines && _originalText.Length > 100 && _finalText.Length > 100)
//         {
//             foreach (var diff in diff_lineMode(_originalText, _finalText, cancellationToken))
//             {
//                 yield return diff;
//             }
//         }

//         foreach (var diff in BisectTexts(_originalText, _finalText, cancellationToken))
//         {
//             yield return diff;
//         }
//     }
// }
