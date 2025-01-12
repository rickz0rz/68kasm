using Common.Amiga;
using Common.M68K.Addresses;

namespace Common.M68K.Instructions;

public class InstructionJsr : BaseInstruction
{
    private BaseInstruction ParentInstruction;

    private const string InstructionName = "JSR";
    private const int InstMask = 0b1111111111000000;
    private const int InstMaskTarget = 0b0100111010000000;

    // for the address referenced, we should check which section it's in and adjust accordingly
    // in the next offsets

    private BaseAddress _source;
    private Hunk _hunk;

    public InstructionJsr(Hunk hunk, int hunkSectionNumber, ref int pc) : base(hunk, hunkSectionNumber, ref pc)
    {
        _hunk = hunk;
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
        _source = InstructionUtilities.ParseSourceAddress(Instruction, hunk, hunkSectionNumber, ref pc,
            ExtraInstructionBytes);

        Console.WriteLine($"[{Address:X6}] JSR -> Jump to {_source} ({_source.GetType()})");

        var addressesToCheck = new List<int>();

        if (_source is ProgramCounterIndirectWithDisplacement pcIndirect)
        {
            var adjusted = Address + 2 + ExtraInstructionBytes.Count + pcIndirect.Displacement;
            // Console.WriteLine($"; [{HunkSectionNumber}] 0x{Address:X6}: Checking PC indirect 0x{adjusted:X8} with displacement tables");
            addressesToCheck.Add(adjusted);
        }
        else if (_source is IndirectWithDisplacement)
        {
            // Calculate the index, and see if it's in the relocation tables too.
            // This might mean attempting to go up the chain to find a direct set to the referenced register
            // that contains an address. If we can't find one, abort.
            // Might have to make a helper function that takes the hunk, hunk section, and current PC
            // and tries to go up the chain...? Or maybe, if I'm traversing the path already, have it return
            // the path of instructions it took to get here. That'd be pretty cool.
        }

        addressesToCheck.Add(pc);
    }

    public override List<SectionAddress> GetNextOffsetAddresses()
    {
        // JSR will return with RTS so add both the computed address
        // and the next offset.
        var offsetAddresses = new List<SectionAddress>
        {
            new()
            {
                SectionNumber = HunkSectionNumber,
                Address = Address + 2 + ExtraInstructionBytes.Count
            }
        };

        if (_source is ProgramCounterIndirectWithDisplacement pcIndirect)
        {
            // I don't think things with 'displacement' get relocations.
            var adjustedDisplacement = Address + 2 + pcIndirect.Displacement;
            offsetAddresses.Add(new SectionAddress
            {
                SectionNumber = HunkSectionNumber,
                Address = adjustedDisplacement
            });
        }

        return offsetAddresses;
    }

    public override string ToAssembly()
    {
        return $"{InstructionName} {_source}";
    }
}
