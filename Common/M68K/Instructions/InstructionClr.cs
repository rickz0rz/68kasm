using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionClr : BaseInstruction
{
    private const string InstructionName = "CLR";
    private const int InstMask = 0b1111111100000000;
    private const int InstMaskTarget = 0b0100001000000000;

    private string _size;
    private string _source;

    public InstructionClr(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
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
        var sizeBits = (Instruction >> 6) & 0b11;
        _size = sizeBits switch
        {
            0b00 => ".B",
            0b01 => ".W",
            0b10 => ".L",
            _ => $".UNDEFINED_{sizeBits:b2}",
        };

        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}{_size} {_source}";
    }
}