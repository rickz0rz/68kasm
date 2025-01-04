using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionSubQ : BaseInstruction
{
    private const string InstructionName = "SUBQ";
    private const int InstMask = 0b1111_0001_0000_0000;
    private const int InstMaskTarget = 0b0101_0001_0000_0000;
    
    private string _size;
    private int _data;
    private string _source;

    public InstructionSubQ(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _size = ((Instruction >> 6) & 0b11) switch
        {
            0b00 => ".B",
            0b01 => ".W",
            0b10 => ".L",
            _ => $".Unknown_{(Instruction >> 6) & 0b11:b2}"
        };

        _data = (Instruction >> 9) & 0b111;
        
        if (_data == 0)
            _data = 8;
        
        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}{_size} #{_data},{_source}";
    }
}