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

    protected BaseInstruction(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        Address = pc;
        Label = null;
        Instruction = (hunk.HunkSections[hunkSectionNumber].Data[pc] << 8) +
                      hunk.HunkSections[hunkSectionNumber].Data[pc + 1];
        pc += 2;

        ExtraInstructionBytes = [];
    }

    public static BaseInstruction FromBytes(Hunk hunk, int hunkSectionNumber, ref int pc)
    {
        // The PC from this is incremented in the instantiation of the instruction so don't
        // increment it here.. think of this as a 'peek'.
        var instruction = (hunk.HunkSections[hunkSectionNumber].Data[pc] << 8) +
                          hunk.HunkSections[hunkSectionNumber].Data[pc + 1];

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

        foreach (var cachedInstruction in _cachedInstructions)
        {
            var isMatchingInstruction = (bool)cachedInstruction.GetMethod("IsInstruction", [typeof(int)])
                .Invoke(null, [instruction]);

            if (!isMatchingInstruction)
                continue;

            var constructorInfo = cachedInstruction.GetConstructor([typeof(Hunk), typeof(int), typeof(int).MakeByRefType()]);

            // This is fun. You have to use an array and copy the value back when you're done
            // if doing an invoke with a ref. See: https://stackoverflow.com/a/8779762
            object[] args = [hunk, hunkSectionNumber, pc];
            var foundInstruction = (BaseInstruction)constructorInfo.Invoke(args);
            pc = (int)args[2];

            foundInstruction.ParseSpecificInstruction(hunk, hunkSectionNumber, ref pc);
            return foundInstruction;
        }

        var parsedInstruction = new BaseInstruction(hunk, hunkSectionNumber, ref pc);
        parsedInstruction.ParseSpecificInstruction(hunk, hunkSectionNumber, ref pc);
        return parsedInstruction;
    }

    //public static BaseInstruction FromAssembly(string assembly)
    //{
    //}

    public virtual List<byte> ToBytes()
    {
        throw new NotImplementedException($"ToMachineCode not implemented yet: {Instruction:X4}");
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