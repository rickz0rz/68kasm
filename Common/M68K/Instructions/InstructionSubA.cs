using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionSubA : BaseInstruction
{
    private const string InstructionName = "SUBA";
    private const int InstMask = 0b1111_0000_0000_0000;
    private const int InstMaskTarget = 0b1001_0000_0000_0000;

    private string _register;
    private string _size;
    private bool _isRegisterFirst;
    private string _source;

    public InstructionSubA(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        var addOpModes = new List<byte> { 0b011, 0b111 };
        return (instruction & InstMask) == InstMaskTarget &&
               addOpModes.Contains((byte)((instruction >> 6) & 0b111));
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _size = ((Instruction >> 6) & 0b111) switch
        {
            0b011 => ".W",
            0b111 => ".L",
            _ => $".Unknown_{(Instruction >> 6) & 0b11:b2}"
        };
        _register = $"A{(Instruction >> 9) & 0b111}";
        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}{_size} {_source},{_register}";
    }
}