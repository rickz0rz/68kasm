namespace Common.Tests.M68K.Instructions;

public class InstructionSubATests
{
    [TestCase("SUBA.L D0,A7", "9FC0")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}