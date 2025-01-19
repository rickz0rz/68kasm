using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionLink : BaseInstruction
{
    private const string InstructionName = "LINK";
    private const int InstMask = 0b1111_1111_1111_1000;
    private const int InstMaskTarget = 0b0100_1110_0101_0000;

    private string _size;
    private BaseAddress _register;
    private int _displacement;

    public InstructionLink(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
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

    public override void ProcessInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        var registerBits = Instruction & 0b111;
        _register = new AddressRegister(registerBits);
        _displacement = InstructionUtilities.ParseTwosComplementWord(hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        // Pretty sure this is only a word, so manually adding the .W
        var formattedDisplacement = InstructionUtilities.FormatValue(_displacement, 8);
        return $"{InstructionName}.W {_register},#{formattedDisplacement}";
    }
}
