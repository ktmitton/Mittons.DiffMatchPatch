using System.Text;
using System.Web;

namespace Mittons.DiffMatchPatch.Benchmark;

/**
    * Class representing one diff operation.
    */
public record Diff(Operation Operation, string Text)
{
    /**
        * Display a human-readable version of this Diff.
        * @return text version.
        */
    public override string ToString()
    {
        string prettyText = Text.Replace('\n', '\u00b6');
        return $"""Diff({Operation},"{prettyText}")""";
    }


    /**
        * Encodes a string with URI-style % escaping.
        * Compatible with JavaScript's encodeURI function.
        *
        * @param str The string to encode.
        * @return The encoded string.
        */
    public string ToUriEncodedString()
    {
        // C# is overzealous in the replacements.  Walk back on a few.
        return new StringBuilder(HttpUtility.UrlEncode(Text))
            .Replace('+', ' ').Replace("%20", " ").Replace("%21", "!")
            .Replace("%2a", "*").Replace("%27", "'").Replace("%28", "(")
            .Replace("%29", ")").Replace("%3b", ";").Replace("%2f", "/")
            .Replace("%3f", "?").Replace("%3a", ":").Replace("%40", "@")
            .Replace("%26", "&").Replace("%3d", "=").Replace("%2b", "+")
            .Replace("%24", "$").Replace("%2c", ",").Replace("%23", "#")
            .Replace("%7e", "~")
            .ToString();
    }

    public string ToDeltaEncodedString2()
    {
        return Operation switch
        {
            Operation.INSERT => $"+{ToUriEncodedString()}\t",
            Operation.DELETE => $"-{Text.Length}\t",
            Operation.EQUAL => $"={Text.Length}\t",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string ToDeltaEncodedString()
    {
        return Operation switch
        {
            Operation.INSERT => $"+{ToUriEncodedString()}",
            Operation.DELETE => $"-{Text.Length}",
            Operation.EQUAL => $"={Text.Length}",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public Diff Append(string value)
    {
        return this with { Text = $"{Text}{value}" };
    }

    public Diff Append(Diff value)
    {
        return this.Append(value.Text);
    }

    public Diff Prepend(string value)
    {
        return this with { Text = $"{value}{Text}" };
    }

    public Diff Prepend(Diff value)
    {
        return this.Prepend(value.Text);
    }
}
