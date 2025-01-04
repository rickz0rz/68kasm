using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionAndI : BaseInstruction
{
    private const string InstructionName = "ANDI";
    private const int InstMask = 0b1111_1111_0000_0000;
    private const int InstMaskTarget = 0b0000_0010_0000_0000;

    private int _value;
    private string _size;
    private string _source;

    public InstructionAndI(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        var sizes = new List<byte> { 0b00, 0b01, 0b10 };
        return (instruction & InstMask) == InstMaskTarget &&
               sizes.Contains((byte)((instruction >> 6) & 0b11));
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
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
        return $"{InstructionName}{_size} #{InstructionUtilities.FormatValue(_value)},{_source}";
    }
}