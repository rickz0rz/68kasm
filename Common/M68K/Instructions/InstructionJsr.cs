using Common.Amiga;

namespace Common.M68K.Instructions;

public class InstructionJsr : BaseInstruction
{
    private const string InstructionName = "JSR";
    private const int InstMask = 0b1111111111000000;
    private const int InstMaskTarget = 0b0100111010000000;

    // for the address referenced, we should check which section it's in and adjust accordingly
    // in the next offsets

    // Change this to an abstract type..?
    private string _source;

    public InstructionJsr(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
    }

    public static bool IsInstruction(string opcode)
    {
        return opcode.Equals(InstructionName, StringComparison.InvariantCultureIgnoreCase);
    }
    
    public static bool IsInstruction(int instruction)
    {
        return (instruction & InstMask) == InstMaskTarget;
    }

    public override void ParseSpecificInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        // Check to see if the current PC exists in the relocation tables for this section.
        // Am I reading the relocation table values correctly? It looks like the ref disasm
        // has section 0 pointed to 1 and 1 pointed to 0 but things end up weird.
        foreach (var tableSectionId in hunk.HunkSections[hunkSectionNumber].RelocationTables.Keys)
        {
            Console.WriteLine($"Checking to see if hunk section {hunkSectionNumber} address 0x{pc:X6} in relocation tables");
            if (hunk.HunkSections[hunkSectionNumber].RelocationTables[tableSectionId].Contains(pc))
            {
                Console.WriteLine($"Warning: hunk section {hunkSectionNumber} address 0x{pc:X6} in relocation " +
                                  $"table {tableSectionId} for section.");
            }
        }

        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);
    }

    public override string ToAssembly()
    {
        return $"{InstructionName} {_source}";
    }
}
