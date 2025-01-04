using Common.Amiga;

namespace Common.M68K.Instructions;

// https://68k.hax.com/Bcc and https://68k.hax.com/BRA
// https://mrjester.hapisan.com/04_MC68/Sect06Part03/Index.html
public class InstructionBcc : BaseInstruction
{
    private const string InstructionNameBra = "BRA";
    private const string InstructionNameBne = "BNE";
    private const string InstructionNameBeq = "BEQ";
    private const int InstMask = 0b11110000000000000;
    private const int InstMaskTarget = 0b0110000000000000;

    private string _instructionName;
    private string _size;
    private int _displacement;
    
    public InstructionBcc(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(string opcode)
    {
        return new[] { InstructionNameBra, InstructionNameBne, InstructionNameBeq }
            .Any(opcode => opcode.Equals(opcode, StringComparison.InvariantCultureIgnoreCase));
    }
    
    public static bool IsInstruction(int instruction)
    {
        return ((instruction & InstMask) == InstMaskTarget) && !InstructionMoveQ.IsInstruction(instruction);
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        var conditionBits = (Instruction >> 8) & 0b1111;
        _instructionName = conditionBits switch
        {
            0b0000 => InstructionNameBra, // Unconditional branch
            0b0110 => InstructionNameBne, // Not equal ... to zero? Do you have to do a CMP first?
            0b0111 => InstructionNameBeq, // Equal
            _ => $"BRCC_UNSUPPORTED_{conditionBits:b4}"
        };
        
        _displacement = (Instruction & 0xFF);
        if (_displacement == 0)
        {
            _size = ".W";
            _displacement = InstructionUtilities.ParseTwosComplementWord(hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes);
        }
        else
        {
            _displacement = InstructionUtilities.ConvertTwosComplementByte(_displacement);
            _size = ".S";
        }
    }

    public override string ToAssembly()
    {
        return $"{_instructionName}{_size} {InstructionUtilities.FormatValue(_displacement)}";
    }
}