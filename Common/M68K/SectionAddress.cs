namespace Common.M68K;

// This should probably go in the amiga specific stuff.
public sealed record SectionAddress
{
    public required int SectionNumber { get; init; }
    public required int Address { get; init; }

    public override string ToString()
    {
        return $"#$[{SectionNumber}]{Address:X8}";
    }
}
