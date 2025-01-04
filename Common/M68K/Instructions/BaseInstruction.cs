using System.Reflection;
using System.Text;
using Common.Amiga;

namespace Common.M68K.Instructions;

public class BaseInstruction
{
    private static readonly List<Type> _cachedInstructions = new List<Type>();
    private const int _instructionLengthBits = 16;

    // Label should probably be a list created post-disassembly before it's printed out.
    public string? Label { get; set; }
    public int Address { get; set; }
    public int Instruction { get; set; }
    public List<byte> ExtraInstructionBytes { get; set; }

    protected BaseInstruction(string assembly)
    {
        
    }

    protected BaseInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        Address = pc;
        Label = null;
        Instruction = (hunk.HunkSections[hunkSectionNumber].Data[pc] << 8) +
                      hunk.HunkSections[hunkSectionNumber].Data[pc + 1];
        pc += 2;

        ExtraInstructionBytes = [];
    }

    private static void CheckForCachedInstructions()
    {
        if (_cachedInstructions.Count == 0)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var list = assembly.GetTypes().Where(t => typeof(BaseInstruction).IsAssignableFrom(t));
                foreach (var type in list)
                {
                    if (type == typeof(BaseInstruction))
                        continue;

                    _cachedInstructions.Add(type);
                }
            }
        }
    }
    
    public static BaseInstruction FromHunk(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        CheckForCachedInstructions();

        // The PC from this is incremented in the instantiation of the instruction so don't
        // increment it here.. think of this as a 'peek'.
        var opcodeBytes = (hunk.HunkSections[hunkSectionNumber].Data[pc] << 8) +
                          hunk.HunkSections[hunkSectionNumber].Data[pc + 1];

        foreach (var cachedInstruction in _cachedInstructions)
        {
            var isMatchingInstruction = (bool)cachedInstruction.GetMethod("IsInstruction", [typeof(int)])
                .Invoke(null, [opcodeBytes]);

            if (!isMatchingInstruction)
                continue;

            var constructorInfo = cachedInstruction.GetConstructor([typeof(Hunk), typeof(int), typeof(int).MakeByRefType()]);

            // This is fun. You have to use an array and copy the value back when you're done
            // if doing an invoke with a ref. See: https://stackoverflow.com/a/8779762
            object[] args = [hunk, hunkSectionNumber, pc];
            var foundInstruction = (BaseInstruction)constructorInfo.Invoke(args);
            pc = (int)args[2];

            // Would be neat to make this somehow work from the base constructor?
            foundInstruction.ParseSpecificInstruction(hunk, hunkSectionNumber, ref pc);
            return foundInstruction;
        }

        var parsedInstruction = new BaseInstruction(hunk, hunkSectionNumber, ref pc);
        parsedInstruction.ParseSpecificInstruction(hunk, hunkSectionNumber, ref pc);
        return parsedInstruction;
    }

    public static BaseInstruction FromAssembly(string assembly)
    {
        CheckForCachedInstructions();
        
        // Split the assembly ; to strip comments, then by whitespace. Trim and
        // grab the first word, the opcode.
        var keywords = assembly.Split(new[] { ' ', '\n', '\t', ';'}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var opcode = keywords.First();
        
        foreach (var cachedInstruction in _cachedInstructions)
        {
            var isMatchingInstruction = (bool)cachedInstruction.GetMethod("IsInstruction", [typeof(string)])
                .Invoke(null, [opcode]);

            if (!isMatchingInstruction)
                continue;

            var constructorInfo = cachedInstruction.GetConstructor([typeof(string[])]);

            // This is fun. You have to use an array and copy the value back when you're done
            // if doing an invoke with a ref. See: https://stackoverflow.com/a/8779762
            object[] args = [keywords];
            var foundInstruction = (BaseInstruction)constructorInfo.Invoke(args);
            // pc = (int)args[2];

            // Would be neat to make this somehow work from the base constructor?
            //foundInstruction.ParseSpecificInstruction(hunk, hunkSectionNumber, ref pc);
            return foundInstruction;
        }

        throw new NotImplementedException($"FromAssembly not implemented yet: {assembly}");
    }

    public virtual List<byte> ToBytes()
    {
        throw new NotImplementedException($"ToBytes not implemented yet: {Instruction:X4}");
    }

    public virtual void ParseSpecificInstruction(Hunk hunk, int hunkSectionId, ref int pc)
    {
        throw new NotImplementedException($"Parse instruction not implemented yet: {Instruction:X4}");
    }

    public virtual string ToAssembly()
    {
        throw new NotImplementedException($"Print instruction not implemented yet: {Instruction:X4}");
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"{ToAssembly()} ;0x{Address:X6}: {Instruction:X4}");
        foreach (var extraInstructionByte in ExtraInstructionBytes)
        {
            stringBuilder.Append($"{extraInstructionByte:X2}");
        }

        return stringBuilder.ToString();
    }
}