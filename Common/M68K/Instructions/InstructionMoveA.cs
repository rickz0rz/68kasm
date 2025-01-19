using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

// https://68k.hax.com/MOVEA
public class InstructionMoveA : BaseInstruction
{
    private const string InstructionName = "MOVEA";
    private const int InstMask = 0b1100000111000000;
    private const int InstMaskTarget = 0b0000000001000000;
    private BaseAddress _source;
    private BaseAddress _destination;

    public InstructionMoveA(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        var sizeModes = new List<byte> { 0b11, 0b10 };
        return (instruction & InstMask) == InstMaskTarget &&
               sizeModes.Contains((byte)((instruction >> 12) & 0b11));
    }

    public override void ProcessInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
        _destination = new Immediate($"A{(Instruction >> 9) & 0b111}");
    }

    public override string ToAssembly()
    {
        // Is this size dynamic? It is... bits 13 and 12. Copy from MoveM?
        return $"{InstructionName}.L {_source},{_destination}";
    }
}
