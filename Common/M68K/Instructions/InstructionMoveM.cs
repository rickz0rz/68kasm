using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public sealed class InstructionMoveM : BaseInstruction
{
    private const string InstructionName = "MOVEM";
    private const int InstMask = 0b1111_1011_1000_0000;
    private const int InstMaskTarget = 0b0100_1000_1000_0000;

    private readonly List<string> _registers;
    private int _dr;
    private string _size;
    private BaseAddress _operationMemoryAddress;
    
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
        // dr, 0 = register to memory (predecrement); 1 = memory to register (postincrement)
        _dr = (Instruction >> 10) & 0b1;

        _operationMemoryAddress =
            InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
                ExtraInstructionBytes);

        ExtraInstructionBytes.Add(hunk.HunkSections[hunkSectionNumber].Data[pc]);
        ExtraInstructionBytes.Add(hunk.HunkSections[hunkSectionNumber].Data[pc + 1]);
        pc += 2;

        var targetByteA = _dr == 0 ? ExtraInstructionBytes[0] : ExtraInstructionBytes[1];
        var targetByteB = _dr == 0 ? ExtraInstructionBytes[1] : ExtraInstructionBytes[0];

        foreach (var pair in new[]
                 {
                     ("D", targetByteA),
                     ("A", targetByteB)
                 })
        {
            var targetRegisterId = pair.Item1;
            var targetRegister = pair.Item2;

            if (_dr == 0)
            {
                ParseMaskPostIncrement(targetRegister, targetRegisterId);
            }
            else
            {
                ParseMaskPreDecrement(targetRegister, targetRegisterId);
            }
        }

        _size = (Instruction & 0b1000000) == 0b1000000 ? "L" : "W";
    }

    private void ParseMaskPostIncrement(byte targetRegister, string targetRegisterId)
    {
        int? firstConsecutive = null;
        for (var i = 8; i >= 0; i--)
        {
            if (i == 8)
            {
                if (!firstConsecutive.HasValue)
                    continue;

                _registers.Add(firstConsecutive.Value != 7
                    ? $"{targetRegisterId}{firstConsecutive.Value}-{targetRegisterId}7"
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

    private void ParseMaskPreDecrement(byte targetRegister, string targetRegisterId)
    {
        int? firstConsecutive = null;
        for (var i = 0; i <= 8; i++)
        {
            if (i == 8)
            {
                if (!firstConsecutive.HasValue)
                    continue;

                _registers.Add(firstConsecutive.Value != 7
                    ? $"{targetRegisterId}{firstConsecutive.Value}-{targetRegisterId}7"
                    : $"{targetRegisterId}7");
            }
            else if (((targetRegister >> i) & 1) == 1)
            {
                firstConsecutive ??= i;
            }
            else
            {
                if (!firstConsecutive.HasValue)
                    continue;

                var lastValue = i - 1;

                _registers.Add(lastValue - firstConsecutive.Value == 0
                    ? $"{targetRegisterId}{firstConsecutive.Value}"
                    : $"{targetRegisterId}{firstConsecutive.Value}-{targetRegisterId}{lastValue}");

                firstConsecutive = null;
            }
        }
    }

    public override string ToAssembly()
    {
        return _dr == 0
            ? $"{InstructionName}.{_size} {string.Join("/", _registers)},{_operationMemoryAddress}"
            : $"{InstructionName}.{_size} {_operationMemoryAddress},{string.Join("/", _registers)}";
    }
}
