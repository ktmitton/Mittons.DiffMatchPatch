namespace Mittons.DiffMatchPatch.Models;

public ref struct CommonMiddleDetails
{
    public ReadOnlySpan<char> LeftPrefix { get; set; }
    public ReadOnlySpan<char> LeftSuffix { get; set; }
    public ReadOnlySpan<char> RightPrefix { get; set; }
    public ReadOnlySpan<char> RightSuffix { get; set; }
    public ReadOnlySpan<char> CommonMiddle { get; set; }

    public readonly bool HasCommonMiddle => CommonMiddle.Length > 0;

    public CommonMiddleDetails(ReadOnlySpan<char> leftPrefix, ReadOnlySpan<char> leftSuffix, ReadOnlySpan<char> rightPrefix, ReadOnlySpan<char> rightSuffix, ReadOnlySpan<char> commonMiddle)
    {
        LeftPrefix = leftPrefix;
        LeftSuffix = leftSuffix;
        RightPrefix = rightPrefix;
        RightSuffix = rightSuffix;
        CommonMiddle = commonMiddle;
    }

    public CommonMiddleDetails()
    {
        LeftPrefix = [];
        LeftSuffix = [];
        RightPrefix = [];
        RightSuffix = [];
        CommonMiddle = [];
    }

    public static bool operator >(CommonMiddleDetails left, CommonMiddleDetails right)
    {
        return left.CommonMiddle.Length > right.CommonMiddle.Length;
    }

    public static bool operator <(CommonMiddleDetails left, CommonMiddleDetails right)
    {
        return left.CommonMiddle.Length < right.CommonMiddle.Length;
    }

    public static bool operator ==(CommonMiddleDetails left, CommonMiddleDetails right)
    {
        return left.LeftPrefix.SequenceEqual(right.LeftPrefix) && left.LeftSuffix.SequenceEqual(right.LeftSuffix) &&
            left.RightPrefix.SequenceEqual(right.RightSuffix) && left.RightSuffix.SequenceEqual(right.RightSuffix) &&
            left.CommonMiddle.SequenceEqual(right.CommonMiddle);
    }

    public static bool operator !=(CommonMiddleDetails left, CommonMiddleDetails right)
    {
        return !(left == right);
    }
}