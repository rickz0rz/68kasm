using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionLsl : BaseInstruction
{
    // Combine this with LSR?
    private const string InstructionName = "LSL";
    private const int InstMask = 0b1111_0001_0000_0000;
    private const int InstMaskTarget = 0b1110_0001_0000_0000;

    private bool _isRegisterShift;
    private string _size;
    private BaseAddress _src;
    private BaseAddress _dest;

    public InstructionLsl(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        // if size = 0b11 then memory shift, otherwise it's a register shift.
        var sizeBits = (Instruction >> 6) & 0b11;
        if (sizeBits == 0b11)
        {
            // memory shift.
            _src = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
                ExtraInstructionBytes);
            _isRegisterShift = false;
        }
        else
        {
            // register shift.
            var ir = (Instruction >> 5) & 0b1;

            _size = sizeBits switch
            {
                0b01 => ".W",
                0b10 => ".L",
                0b00 => ".B",
                _ => $".Unknown_{sizeBits:b2}"
            };

            var srcBits = (Instruction >> 9) & 0b111;
            var srcCount = srcBits == 0 ? 8 : srcBits;
            _src = new GenericString($"#{srcCount}");
            _dest = new GenericString($"D{Instruction & 0b111}");
            _isRegisterShift = true;
        }
    }

    public override string ToAssembly()
    {
        return _isRegisterShift
            ? $"{InstructionName}{_size} {_src},{_dest}"
            : $"{InstructionName} {_src}";
    }
}
