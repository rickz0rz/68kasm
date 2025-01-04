using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionJsr : BaseInstruction
{
    private const string InstructionName = "JSR";
    private const int InstMask = 0b1111111111000000;
    private const int InstMaskTarget = 0b0100111010000000;

    // Change this to an abstract type..?
    private string _source;

    public InstructionJsr(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(string opcode)
    {
        return opcode.Equals(InstructionName, StringComparison.InvariantCultureIgnoreCase);
    }
    
    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        return $"{InstructionName} {_source}";
    }
}