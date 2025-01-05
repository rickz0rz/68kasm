using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionRts : BaseInstruction
{
    private const string InstructionName = "RTS";
    private const int InstMask = 0b1111_1111_1111_1111;
    private const int InstMaskTarget = 0b0100_1110_0111_0101;

    public InstructionRts(string assembly) : base(assembly)
    {
    }

    public InstructionRts(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
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
        return [0x4E, 75];
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}";
    }
}
