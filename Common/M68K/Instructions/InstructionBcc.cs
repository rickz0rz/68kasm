using Common.Amiga;

namespace Common.M68K.Instructions;

// https://68k.hax.com/Bcc and https://68k.hax.com/BRA
// https://mrjester.hapisan.com/04_MC68/Sect06Part03/Index.html
public class InstructionBcc : BaseInstruction
{
    private enum BccInstructionVariation
    {
        BRA,
        BCC,
        BCS,
        BEQ,
        BGE,
        BGT,
        BHI,
        BLE,
        BLS,
        BLT,
        BMI,
        BNE,
        BPL,
        BVC,
        BVS
    };

    private const int InstMask = 0b11110000000000000;
    private const int InstMaskTarget = 0b0110000000000000;

    private BccInstructionVariation _instructionName;
    private string _size;
    private int _displacement;
    
    public InstructionBcc(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(string opcode)
    {
        return Enum.GetValues<BccInstructionVariation>()
            .Any(variation => opcode.Equals(variation.ToString(), StringComparison.OrdinalIgnoreCase));
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
            0b0000 => BccInstructionVariation.BRA, // Unconditional branch
            0b0100 => BccInstructionVariation.BCC, // Carry clear
            0b0111 => BccInstructionVariation.BEQ, // Equal
            0b1100 => BccInstructionVariation.BGE, // Greater/equal
            0b0010 => BccInstructionVariation.BHI, // High
            0b1111 => BccInstructionVariation.BLE, // Less/equal
            0b0011 => BccInstructionVariation.BLS, // Low or same
            0b1101 => BccInstructionVariation.BLT, // Less than
            0b1011 => BccInstructionVariation.BMI, // Minus
            0b0110 => BccInstructionVariation.BNE, // Not equal
            0b1010 => BccInstructionVariation.BPL, // Plus
            0b1000 => BccInstructionVariation.BVC, // Overflow clear
            0b1001 => BccInstructionVariation.BVS, // Overflow set
            _ => throw new Exception($"Unhandled condition bits: {conditionBits:b4}")
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

        Console.WriteLine($"[{Address:X6}] {_instructionName} -> Branch to 0x{_displacement:X6}");
    }

    public override List<SectionAddress> GetNextOffsetAddresses()
    {
        var addresses = new List<SectionAddress>();

        // Address + offset.
        var jumpOffset = Address + 2 + ExtraInstructionBytes.Count + _displacement;
        // Console.WriteLine($"; [{HunkSectionNumber}] 0x{Address:X6}: {_instructionName}: Processing jump offset 0x{jumpOffset:X8}");
        addresses.Add(new SectionAddress()
        {
            SectionNumber = HunkSectionNumber,
            Address = jumpOffset
        });

        // BRA is "always branch", so we'll never fall-through.
        if (_instructionName != BccInstructionVariation.BRA)
        {
            // Since this is just a fall-through from a branch, don't add it as a label.
            // var fallThroughOffset = Address + 2 + ExtraInstructionBytes.Count;
            // Console.WriteLine($"; [{HunkSectionNumber}] 0x{Address:X6}: {_instructionName}: Processing fall-through offset 0x{fallThroughOffset:X8}");
            addresses.Add(new SectionAddress()
            {
                SectionNumber = HunkSectionNumber,
                Address = Address + 2 + ExtraInstructionBytes.Count
            });
        }

        return addresses;
    }

    public override string ToAssembly()
    {
        return $"{_instructionName}{_size} {InstructionUtilities.FormatValue(_displacement)}";
    }
}
