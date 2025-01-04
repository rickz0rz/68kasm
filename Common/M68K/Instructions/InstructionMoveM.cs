using Common.Amiga;

namespace Common.M68K.Instructions;

public sealed class InstructionMoveM : BaseInstruction
{
    private const string InstructionName = "MOVEM";
    private const int InstMask = 0b1111111110000000;
    private const int InstMaskTarget = 0b0100100010000000;

    private readonly List<string> _registers;
    private string _size;
    private string _operationMemoryAddress;
    
    public InstructionMoveM(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
        _registers = [];
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        var operationMemoryMode = (Instruction >> 3) & 7;
        var operationMemoryAddress = (Instruction & 7).ToString();
        _operationMemoryAddress = operationMemoryMode switch
        {
            0b100 => $"-(A{operationMemoryAddress})",
            0b010 => $"(A{operationMemoryAddress})",
            _ => operationMemoryAddress
        };
        
        ExtraInstructionBytes.Add(hunk.HunkSections[hunkSectionNumber].Data[pc]);
        ExtraInstructionBytes.Add(hunk.HunkSections[hunkSectionNumber].Data[pc + 1]);
        pc += 2;

        foreach (var pair in new[] { ("D", ExtraInstructionBytes[0]), ("A", ExtraInstructionBytes[1]) })
        {
            var targetRegisterId = pair.Item1;
            var targetRegister = pair.Item2;
            
            // This is pretty messy.
            int? firstConsecutive = null;
            for (var i = 8; i >= 0; i--)
            {
                if (i == 8)
                {
                    if (!firstConsecutive.HasValue)
                        continue;

                    _registers.Add(firstConsecutive.Value != 7
                        ? $"{targetRegisterId}{firstConsecutive.Value}-D7"
                        : $"{targetRegisterId}7");
                }
                else if (((targetRegister >> i) & 1) == 1)
                {
                    firstConsecutive ??= 7 - i;
                }
                else
                {
                    if (!firstConsecutive.HasValue)
                        continue;
                    
                    var lastValue = 7 - i - 1;

                    _registers.Add(lastValue - firstConsecutive.Value == 0
                        ? $"{targetRegisterId}{firstConsecutive.Value}"
                        : $"{targetRegisterId}{firstConsecutive.Value}-{targetRegisterId}{lastValue}");
                }
            }
        }
        
        _size = (Instruction & 0b1000000) == 0b1000000 ? "L" : "W";
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}.{_size} {string.Join("/", _registers)},{_operationMemoryAddress}";
    }
}