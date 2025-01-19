using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionUnlk : BaseInstruction
{
    private const string InstructionName = "UNLK";
    private const int InstMask = 0b1111_1111_1111_1000;
    private const int InstMaskTarget = 0b0100_1110_0101_1000;

    private AddressRegister _register;

    public InstructionUnlk(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ProcessInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        var registerBits = Instruction & 0b111;
        _register = new AddressRegister(registerBits);
    }

    public override string ToAssembly()
    {
        return $"{InstructionName} {_register}";
    }
}
