using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionTst : BaseInstruction
{
    private const string InstructionName = "TST";
    private const int InstMask = 0b1111_1111_0000_0000;
    private const int InstMaskTarget = 0b0100_1010_0000_0000;

    private string _size;
    private string _source;

    public InstructionTst(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _size = ((Instruction >> 6) & 0b11) switch
        {
            0b00 => ".B",
            0b01 => ".W",
            0b10 => ".L",
            _ => $".Unknown_{(Instruction >> 6) & 0b11:b2}"
        };
        
        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}{_size} {_source}";
    }
}