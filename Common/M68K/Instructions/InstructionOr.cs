using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionOr : BaseInstruction
{
    private const string InstructionName = "OR";
    private const int InstMask = 0b1111_0000_0000_0000;
    private const int InstMaskTarget = 0b1000_0000_0000_0000;

    // Or kind of matches path of add?

    private BaseAddress _source;
    private BaseAddress _register;
    private string _size;
    private bool _destinationFirst;

    private int _precision = 1; // backport this to andi

    public InstructionOr(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        var addOpModes = new List<byte> { 0b000, 0b001, 0b010, 0b100, 0b101, 0b110 };
        return (instruction & InstMask) == InstMaskTarget &&
               addOpModes.Contains((byte)((instruction >> 6) & 0b111));
    }

    public override void ProcessInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        var registerBits = (Instruction >> 9) & 0b111;
        _register = new DataRegister(registerBits);

        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);

        var opmodeBits = (Instruction >> 6) & 0b111;

        _size = (opmodeBits & 0b11) switch
        {
            0b00 => ".B",
            0b01 => ".W",
            0b10 => ".L",
            _ => $".Unknown_{opmodeBits:b2}"
        };

        _destinationFirst = (opmodeBits & 0b100) == 0b100;
    }

    public override string ToAssembly()
    {
        return _destinationFirst
            ? $"{InstructionName}{_size} {_register},{_source}"
            : $"{InstructionName}{_size} {_source},{_register}";
    }
}
