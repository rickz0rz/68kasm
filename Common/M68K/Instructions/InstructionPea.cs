using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionPea : BaseInstruction
{
    private const string InstructionName = "PEA";
    private const int InstMask = 0b1111_1111_1100_0000;
    private const int InstMaskTarget = 0b0100_1000_0100_0000;
    
    private string _src;

    public InstructionPea(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _src = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        return $"{InstructionName} {_src}";
    }
}