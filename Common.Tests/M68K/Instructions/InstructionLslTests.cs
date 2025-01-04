namespace Common.Tests.M68K.Instructions;

public class InstructionLslTests
{
    [TestCase("LSL.L #2,D0", "E588")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}