using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionCmpI : BaseInstruction
{
    private const string InstructionName = "CMPI";
    private const int InstMask = 0b1111_1111_0000_0000;
    private const int InstMaskTarget = 0b0000_1100_0000_0000;

    private Hunk _hunk;
    private BaseAddress _register;
    private int _value;
    private string _size;

    public InstructionCmpI(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
        _hunk = hunk;
    }

    public static bool IsInstruction(string opcode)
    {
        return opcode.Equals(InstructionName, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsInstruction(int instruction)
    {
        var allowedSizes = new List<int> { 0b00, 0b01, 0b10 };
        var size = (instruction >> 6) & 0b11;
        return (instruction & InstMask) == InstMaskTarget && allowedSizes.Contains(size);
    }

    public override void ProcessInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _register = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);

        var sizeBits = (Instruction >> 6) & 0b11;
        switch (sizeBits)
        {
            case 0b00:
                _size = ".B";
                throw new NotImplementedException();
                break;
            case 0b01:
                _size = ".W";
                _value = InstructionUtilities.ParseWord(hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes);
                break;
            case 0b10:
                _size = ".L";
                _value = InstructionUtilities.ParseLongWord(hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes);
                break;
            default:
                _size = $".Unknown_{sizeBits:b2}";
                break;
        }
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}{_size} #${_value:X8},{_register}";
    }
}
