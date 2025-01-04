using Common.Amiga;

namespace Common.M68K.Instructions;

// https://68k.hax.com/MOVEA
public class InstructionMoveA : BaseInstruction
{
    private const string InstructionName = "MOVEA";
    private const int InstMask = 0b1100000111000000;
    private const int InstMaskTarget = 0b0000000001000000;
    private string _source;
    private string _destRegister;

    public InstructionMoveA(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        var sizeModes = new List<byte> { 0b11, 0b10 };
        return (instruction & InstMask) == InstMaskTarget &&
               sizeModes.Contains((byte)((instruction >> 12) & 0b11));
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        /*
        var sourceAddressingMode = (Instruction >> 3) & 0b111;
        _source = sourceAddressingMode switch
        {
            0b000 => $"D{Instruction & 0b111}",
            0b001 => $"A{Instruction & 0b111}",
            0b111 => InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
                ExtraInstructionBytes),
            _ => $"D-UnknownSrcAddrMode_{sourceAddressingMode:X}"
        };

        _destRegister = $"A{(Instruction >> 9) & 0b111}";
        */

        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
        _destRegister = $"A{(Instruction >> 9) & 0b111}";
    }

    public override string ToAssembly()
    {
        // Is this size dynamic? It is... bits 13 and 12. Copy from MoveM?
        return $"{InstructionName}.L {_source},{_destRegister}";
    }
}