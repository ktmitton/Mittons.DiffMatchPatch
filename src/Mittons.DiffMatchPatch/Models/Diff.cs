using Mittons.DiffMatchPatch.Types;

namespace Mittons.DiffMatchPatch.Models;

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