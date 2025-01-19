using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionCmp : BaseInstruction
{
    private const string InstructionName = "CMP";
    private const int InstMask = 0b1111_0000_0000_0000;
    private const int InstMaskTarget = 0b1011_0000_0000_0000;

    private string _size;
    private BaseAddress _src;
    private DataRegister _dataRegister;

    public InstructionCmp(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(string opcode)
    {
        return opcode.Equals(InstructionName, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsInstruction(int instruction)
    {
        var allowedSizes = new List<int> { 0b000, 0b001, 0b010 };
        var size = (instruction >> 6) & 0b111;
        return (instruction & InstMask) == InstMaskTarget && allowedSizes.Contains(size);
    }

    public override void ProcessInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _src = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);

        var registerBits = (Instruction >> 9) & 0b111;
        _dataRegister = new DataRegister(registerBits);

        var sizeBits = (Instruction >> 6) & 0b11;
        _size = sizeBits switch
        {
            0b000 => ".B",
            0b001 => ".W",
            0b010 => ".L",
            _ => $".Unknown_{sizeBits:b2}"
        };
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}{_size} {_src},{_dataRegister}";
    }
}
