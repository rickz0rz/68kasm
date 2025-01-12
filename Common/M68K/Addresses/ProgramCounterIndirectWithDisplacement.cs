namespace Common.M68K.Addresses;

public class ProgramCounterIndirectWithDisplacement : BaseAddress
{
    public int HunkSectionId { get; init; }
    public int Displacement { get; init; }

    public ProgramCounterIndirectWithDisplacement(int hunkSectionId, int displacement) : base()
    {
        HunkSectionId = hunkSectionId;
        Displacement = displacement;
    }

    public override string ToString()
    {
        return $"$[{HunkSectionId}]{Displacement:X}(PC)";
    }
}
