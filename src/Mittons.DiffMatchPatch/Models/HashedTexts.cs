namespace Mittons.DiffMatchPatch.Models;

public record HashedTexts(List<string> HashLookup, string Text1Hashed, string Text2Hashed, int HashLookupLength = 0);
