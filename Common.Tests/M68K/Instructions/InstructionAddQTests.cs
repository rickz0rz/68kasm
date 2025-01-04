namespace Common.Tests.M68K.Instructions;

public class InstructionAddQTests
{
    [TestCase("ADDQ.L #1,D0", "5280")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}