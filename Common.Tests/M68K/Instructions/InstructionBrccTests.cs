using System.Reflection;

namespace Common.Tests.M68K.Instructions;

public class InstructionBrccTests
{
    [TestCase("BRA.S 2", "6002")]
    [TestCase("BNE.S 6", "6606")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}