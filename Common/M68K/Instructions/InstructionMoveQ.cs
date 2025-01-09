using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

// https://68k.hax.com/MOVEQ
public class InstructionMoveQ : BaseInstruction
{
    private const string InstructionName = "MOVEQ";
    private const int InstMask = 0b1111000100000000;
    private const int InstMaskTarget = 0b0111000000000000;
    
    private int _value;
    private BaseAddress _destRegister;
    
    public InstructionMoveQ(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _destRegister = new GenericStringAddress($"D{Instruction >> 9 & 0b111}");
        _value = Instruction & 0xFF; // this may be two's compliment.
    }

    public override string ToAssembly()
    {
        return $"{InstructionName} #{InstructionUtilities.FormatValue(_value)},{_destRegister}";
    }
}
