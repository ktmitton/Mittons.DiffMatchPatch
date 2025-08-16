namespace Mittons.DiffMatchPatch.Types;

/// <summary>
/// The types of operations for a Diff.
/// </summary>
public enum Operation
{
    DELETE, INSERT, EQUAL
}

public static class CharExtensions
{
    public static Operation ToOperation(this char identifier)
    {
        return identifier switch
        {
            '-' => Operation.DELETE,
            '+' => Operation.INSERT,
            '=' => Operation.EQUAL,
            _ => throw new ArgumentOutOfRangeException(nameof(identifier), identifier, null)
        };
    }
}
