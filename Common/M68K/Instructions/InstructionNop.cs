using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionNop : BaseInstruction
{
    private const string InstructionName = "NOP";
    private const int InstMask = 0b1111_1111_1111_1111;
    private const int InstMaskTarget = 0b0100_1110_0111_0001;

    public InstructionNop(string assembly) : base(assembly)
    {
    }

    public InstructionNop(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
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

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
    }

    public override List<byte> ToBytes()
    {
        return [0x4E, 0x71];
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}";
    }
}