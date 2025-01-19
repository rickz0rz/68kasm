using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

// DBcc = Decrement and Branch, Conditionally
// https://68k.hax.com/DBccDBRA
public class InstructionDbcc : BaseInstruction
{
    private enum DbccInstructionVariation
    {
        DBF
    };

    private const int InstMask = 0b1111_0000_1111_1000;
    private const int InstMaskTarget = 0b0101_0000_1100_1000;
    
    private DbccInstructionVariation _dbccInstructionName;
    private BaseAddress _counterRegister;
    private BaseAddress _displacement;

    public InstructionDbcc(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(string opcode)
    {
        return Enum.GetValues<DbccInstructionVariation>()
            .Any(variation => opcode.Equals(variation.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ProcessInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        var conditionBits = (Instruction >> 8) & 0b1111;
        _dbccInstructionName = conditionBits switch
        {
            1 => DbccInstructionVariation.DBF,
            _ => throw new Exception($"Unhandled condition bits: {conditionBits:b4}")
        };
        
        _counterRegister = new GenericString($"D{Instruction & 0b111}");
        _displacement =
            new GenericString(InstructionUtilities.ParseTwosComplementWord(hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes).ToString());
    }

    public override string ToAssembly()
    {
        return $"{_dbccInstructionName} {_counterRegister},{_displacement}";
    }
}
