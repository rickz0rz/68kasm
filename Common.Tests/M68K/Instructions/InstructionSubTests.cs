namespace Common.Tests.M68K.Instructions;

public class InstructionSubTests
{
    [TestCase("SUB.L 4(A7),D0", "90AF0004")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}