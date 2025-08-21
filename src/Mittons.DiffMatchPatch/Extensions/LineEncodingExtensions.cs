// using Mittons.DiffMatchPatch.Models;

// namespace Mittons.DiffMatchPatch.Extensions;

// public static class LineEncoder
// {
//     public static string[] Encode(IList<string> decodeLookup, params string[] texts)
//     {
//         Dictionary<string, int> encodeLookup = new();

//         texts.Select(text =>
//         {
//             foreach (var line in text.AsSpan().Split('\n'))
//             {
//                 if (line.Length == 0)
//                 {
//                     return string.Empty;
//                 }
//             }

//             .Select(line =>
//             {
//                 if (line.Length == 0)
//                 {
//                     return string.Empty;
//                 }

//                 if (!encodeLookup.TryGetValue(line, out var index))
//                 {
//                     index = decodeLookup.Count;
//                     decodeLookup.Add(line);
//                     encodeLookup[line] = index;
//                 }

//                 return ((char)index).ToString();
//             }).ToArray();

//             if (text.Length == 0)
//             {
//                 return string.Empty;
//             }

//             if (!encodeLookup.TryGetValue(text, out var index))
//             {
//                 index = decodeLookup.Count;
//                 decodeLookup.Add(text);
//                 encodeLookup[text] = index;
//             }

//             return ((char)index).ToString();
//         });

//         foreach (var text in texts)
//         {
//             if (text.Length == 0)
//             {
//                 continue;
//             }
//         }

//         ReadOnlySpan<char> temp = [];
//         var a = temp.Split('\n');

//         // Allocate 2/3rds of the space for text1, the rest for text2.
//         IEnumerable<char> chars1 = CompressLines(text1, decodeLookup, encodeLookup, 40000);
//         IEnumerable<char> chars2 = CompressLines(text2, decodeLookup, encodeLookup, 65536);

//         return new(decodeLookup, new string([.. chars1]), new string([.. chars2]));
//     }

//     /**
//      * Split two texts into a list of strings.  Reduce the texts to a string of
//      * hashes where each Unicode character represents one line.
//      * @param text1 First string.
//      * @param text2 Second string.
//      * @return Three element Object array, containing the encoded text1, the
//      *     encoded text2 and the List of unique strings.  The zeroth element
//      *     of the List of unique strings is intentionally blank.
//      */
//     public static HashedTexts CompressTexts(this string text1, string text2)
//     {
//         List<string> lineArray = [];
//         Dictionary<string, int> lineHash = [];

//         // Allocate 2/3rds of the space for text1, the rest for text2.
//         IEnumerable<char> chars1 = CompressLines(text1, lineArray, lineHash, 40000);
//         IEnumerable<char> chars2 = CompressLines(text2, lineArray, lineHash, 65536);

//         return new(lineArray, new string([.. chars1]), new string([.. chars2]));
//     }

//     /**
//      * Split a text into a list of strings.  Reduce the texts to a string of
//      * hashes where each Unicode character represents one line.
//      * @param text String to encode.
//      * @param lineArray List of unique strings.
//      * @param lineHash Map of strings to indices.
//      * @param maxLines Maximum length of lineArray.
//      * @return Encoded string.
//      */
//     public static IEnumerable<char> CompressLines(string text, List<string> lineArray, Dictionary<string, int> lineHashes, int maxLines)
//     {
//         int lineStart = 0;
//         int lineEnd = -1;
//         string line;

//         while ((lineEnd < text.Length - 1) && lineArray.Count < (maxLines + 1))
//         {
//             lineEnd = text.IndexOf('\n', lineStart);
//             if (lineEnd == -1)
//             {
//                 lineEnd = text.Length - 1;
//             }
//             line = text[lineStart..(lineEnd + 1)];

//             if (!lineHashes.TryGetValue(line, out var hash))
//             {
//                 if (lineArray.Count == maxLines)
//                 {
//                     line = text[lineStart..];
//                     lineEnd = text.Length;
//                 }
//                 hash = lineArray.Count;
//                 lineHashes[line] = hash;
//                 lineArray.Add(line);
//             }

//             lineStart = lineEnd + 1;

//             yield return (char)hash;
//         }
//     }
// }
