using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionOrI : BaseInstruction
{
    private const string InstructionName = "ORI";
    private const int InstMask = 0b1111_1111_0000_0000;
    private const int InstMaskTarget = 0b0000_0000_0000_0000;

    private BaseAddress _source;
    private string _size;
    private int _value;
    private int _precision = 1; // backport this to andi

    public InstructionOrI(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        // AndI and OrI are very much alike here.
        var sizes = new List<byte> { 0b00, 0b01, 0b10 };
        return (instruction & InstMask) == InstMaskTarget &&
               sizes.Contains((byte)((instruction >> 6) & 0b11));
    }

    public override void ProcessInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
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
                _precision = 4;
                _value = InstructionUtilities.ParseWord(hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes);
                break;
            case 0b10:
                _size = ".L";
                _precision = 8;
                _value = InstructionUtilities.ParseLongWord(hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes);
                break;
            default:
                _size = $".Unknown_{sizeBits:b2}";
                break;
        }
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}{_size} #{InstructionUtilities.FormatValue(_value, _precision)},{_source}";
    }
}
