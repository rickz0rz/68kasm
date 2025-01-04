using Common.Amiga;

namespace Common.Tests;

public static class TestHelpers
{
    public static void RunFullTest(string assembly, string bytesString)
    {
        if (assembly.Equals("NOP", StringComparison.InvariantCultureIgnoreCase))
        {
            RunAssemblyTest(assembly, bytesString);
        }
        
        RunDisassemblyTest(assembly, bytesString);
    }

    private static void RunAssemblyTest(string assembly, string bytesString)
    {
        var instruction = Common.M68K.Instructions.BaseInstruction.FromAssembly(assembly);
        var generatedByteString = Convert.ToHexString(instruction.ToBytes().ToArray());
        Assert.That(generatedByteString, Is.EqualTo(bytesString));
    }

    private static void RunDisassemblyTest(string assembly, string bytesString)
    {
        // Disassemble.
        var hunk = new Hunk
        {
            HunkSections =
            {
                new HunkSection
                {
                    Data = ToByteArray(bytesString).ToList()
                }
            }
        };

        var pc = 0;
        var instruction = Common.M68K.Instructions.BaseInstruction.FromHunk(hunk, 0, ref pc);
        
        Assert.That(instruction.ToAssembly(), Is.EqualTo(assembly));
        Assert.That(instruction.ToString(), Is.EqualTo($"{assembly} ;0x000000: {bytesString}"));
    }
    
    private static byte[] ToByteArray(string hex)
    {
        var length = hex.Length / 2;
        var result = new byte[length];
		
        for (var i = 0; i < length; i++) 
        {
            result[i] = Convert.ToByte(hex.Substring(i * 2,2), 16);
        }
		
        return result;
    }
}