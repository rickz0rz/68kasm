using Common.Amiga;

namespace Common.M68K.Instructions;

// DBcc = Decrement and Branch, Conditionally
// https://68k.hax.com/DBccDBRA
public class InstructionDbcc : BaseInstruction
{
    private const string InstructionNameDbf = "DBF";
    private const int InstMask = 0b1111000011111000;
    private const int InstMaskTarget = 0b0101000011001000;
    
    private string _instructionName;
    private string _counterRegister;
    private int _displacement;

    public InstructionDbcc(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(string opcode)
    {
        return new[] { InstructionNameDbf }
            .Any(opcode => opcode.Equals(opcode, StringComparison.InvariantCultureIgnoreCase));
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        var conditionBits = (Instruction >> 8) & 0b1111;
        _instructionName = conditionBits switch
        {
            1 => InstructionNameDbf,
            _ => $"Unhandled_{conditionBits:b4}"
        };
        
        _counterRegister = $"D{Instruction & 0b111}";
        _displacement =
            InstructionUtilities.ParseTwosComplementWord(hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        return $"{_instructionName} {_counterRegister},{_displacement}";
    }
}
