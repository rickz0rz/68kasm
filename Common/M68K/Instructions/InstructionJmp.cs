using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionJmp : BaseInstruction
{
    private const string InstructionName = "JMP";
    private const int InstMask = 0b1111_1111_1100_0000;
    private const int InstMaskTarget = 0b0100_1110_1100_0000;

    private Hunk _hunk;
    private BaseAddress _src;

    public InstructionJmp(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
        _hunk = hunk;
    }

    public static bool IsInstruction(string opcode)
    {
        return opcode.Equals(InstructionName, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _src = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
    }

    // Before I do this, I need to parse out the address into a int value
    public override List<SectionAddress> GetNextOffsetAddresses()
    {
        var result = new List<SectionAddress>();

        if (_src is AbsoluteAddress absoluteAddress)
        {
            result.Add(absoluteAddress.SectionAddress);
        }

        return result;
    }

    public override string ToAssembly()
    {
        return $"{InstructionName} {_src}";
    }
}
