using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionClr : BaseInstruction
{
    private const string InstructionName = "CLR";
    private const int InstMask = 0b1111_1111_0000_0000;
    private const int InstMaskTarget = 0b0100_0010_0000_0000;

    private string _size;
    private BaseAddress _source;

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
