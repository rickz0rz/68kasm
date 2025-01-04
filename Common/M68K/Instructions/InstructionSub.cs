using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionSub : BaseInstruction
{
    private const string InstructionName = "SUB";
    private const int InstMask = 0b1111_0000_0000_0000;
    private const int InstMaskTarget = 0b1001_0000_0000_0000;

    private string _register;
    private string _size;
    private bool _isRegisterFirst;
    private string _source;

    public InstructionSub(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        var addOpModes = new List<byte> { 0b000, 0b001, 0b010, 0b100, 0b101, 0b110 };
        return (instruction & InstMask) == InstMaskTarget &&
               addOpModes.Contains((byte)((instruction >> 6) & 0b111));
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _register = $"D{Instruction >> 9 & 3}";
        
        _size = ((Instruction >> 6) & 0b11) switch
        {
            0b00 => ".B",
            0b01 => ".W",
            0b10 => ".L",
            _ => $".Unknown_{(Instruction >> 6) & 0b11:b2}"
        };

        switch ((Instruction >> 6) & 0b111)
        {
            case 0b100:
                _size = ".B";
                _isRegisterFirst = false;
                break;
            case 0b000:
                _size = ".B";
                _isRegisterFirst = true;
                break;
            case 0b101:
                _size = ".W";
                _isRegisterFirst = false;
                break;
            case 0b001:
                _size = ".W";
                _isRegisterFirst = true;
                break;
            case 0b110:
                _size = ".L";
                _isRegisterFirst = false;
                break;
            case 0b010:
                _size = ".L";
                _isRegisterFirst = true;
                break;
            default:
                _size = $".Unknown_{((Instruction >> 6) & 0b111):b3}";
                break;
        }
        
        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        var formattedSource = _isRegisterFirst ? $"{_source},{_register}" : $"{_register},{_source}";
        return $"{InstructionName}{_size} {formattedSource}";
    }
}