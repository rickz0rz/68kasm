using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionAddA : BaseInstruction
{
    private const string InstructionName = "ADDA";
    private const int InstMask = 0b1111_0000_0000_0000;
    private const int InstMaskTarget = 0b1101_0000_0000_0000;

    private BaseAddress _register;
    private string _size;
    private bool _isRegisterFirst;
    private BaseAddress _source;

    public InstructionAddA(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        var addOpModes = new List<byte> { 0b011, 0b111 };
        return (instruction & InstMask) == InstMaskTarget &&
               addOpModes.Contains((byte)((instruction >> 6) & 0b111));
    }

    public override void ProcessInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _size = ((Instruction >> 6) & 0b111) switch
        {
            0b011 => ".W",
            0b111 => ".L",
            _ => $".Unknown_{(Instruction >> 6) & 0b11:b2}"
        };
        _register = new GenericString($"A{(Instruction >> 9) & 0b111}");
        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}{_size} {_source},{_register}";
    }
}
