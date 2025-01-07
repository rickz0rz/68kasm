namespace Common.M68K;

public record SectionOffset
{
    public required int HunkSectionNumber { get; init; }
    public required int Offset { get; init; }
}
