using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionAddI : BaseInstruction
{
    private const string InstructionName = "ADDI";
    private const int InstMask = 0b1111_1111_0000_0000;
    private const int InstMaskTarget = 0b0000_0110_0000_0000;
    
    private string _size;
    private BaseAddress _register;
    private string _data;

    public InstructionAddI(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _register = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);

        var sizeBits = (Instruction >> 6) & 0b11;
        switch (sizeBits)
        {
            case 0b00:
                _size = ".B";
                // byte
                break;
            case 0b01:
                _size = ".W";
                // word
                break;
            case 0b10:
                _size = ".L";
                _data = $"#${InstructionUtilities.ParseLongWord(hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes):X8}";
                break;
            default:
                _size = $".Unknown_{sizeBits:b2}";
                break;
        }
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}{_size} {_data},{_register}";
    }
}
