using System.Reflection;

namespace Common.Tests.M68K.Instructions;

public class InstructionBccTests
{
    [TestCase("BRA.S 2", "6002")]
    [TestCase("BNE.S 6", "6606")]
    [TestCase("BGE.S 8", "6C08")]
    [TestCase("BRA.W LAB_4324C", "6000FF72")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
