using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionLsl : BaseInstruction
{
    // Combine this with LSR?
    private const string InstructionName = "LSL";
    private const int InstMask = 0b1111_0001_0000_0000;
    private const int InstMaskTarget = 0b1110_0001_0000_0000;
    
    private string _destRegister;
    private string _srcAddress;

    public InstructionLsl(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _destRegister = $"A{(Instruction >> 9) & 0b111}";

        var addressingMode = (Instruction >> 3) & 0b111;
        var addressingModeRegister = Instruction & 0b111;

        switch (addressingMode)
        {
            case 0b111 when (addressingModeRegister & 0b001) == 0b001:
                // Read 4 bytes in
                ExtraInstructionBytes.Add(hunk.HunkSections[hunkSectionNumber].Data[pc + 0]);
                ExtraInstructionBytes.Add(hunk.HunkSections[hunkSectionNumber].Data[pc + 1]);
                ExtraInstructionBytes.Add(hunk.HunkSections[hunkSectionNumber].Data[pc + 2]);
                ExtraInstructionBytes.Add(hunk.HunkSections[hunkSectionNumber].Data[pc + 3]);
                pc += 4;

                // Convert that to an address.
                var address = ExtraInstructionBytes[0] * 256 * 256 * 256 +
                              ExtraInstructionBytes[1] * 256 * 256 +
                              ExtraInstructionBytes[2] * 256 +
                              ExtraInstructionBytes[3];

                _srcAddress = $"${address:X8}";
                break;
            default:
                _srcAddress = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc, ExtraInstructionBytes);
                break;
        }
    }

    public override string ToAssembly()
    {
        return $"{InstructionName} {_srcAddress},{_destRegister}";
    }
}