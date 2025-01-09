using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionMove : BaseInstruction
{
    private const string InstructionName = "MOVE";
    private const int InstMask = 0b1100_0000_0000_0000;
    private const int InstMaskTarget = 0b0000_0000_0000_0000;
    
    private string _size;
    private BaseAddress _src;
    private BaseAddress _dest;
    
    public InstructionMove(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }
    
    public static bool IsInstruction(string opcode)
    {
        return opcode.Equals(InstructionName, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsInstruction(int instruction)
    {
        var sizeModes = new List<byte> { 0b01, 0b11, 0b10 };
        return (instruction & InstMask) == InstMaskTarget
            && !InstructionMoveA.IsInstruction(instruction) // Has bits 001 in 876
            && sizeModes.Contains((byte)((instruction >> 12) & 0b11));
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        _size = InstructionUtilities.ParseSize(Instruction);

        var sizeBits = (Instruction >> 12) & 0b11;
        
        _src = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes, sizeBits);
        _dest = InstructionUtilities.ParseDestinationAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        return $"{InstructionName}{_size} {_src},{_dest}";
    }
}
