namespace Common.Tests.M68K.Instructions;

public class InstructionSubQTests
{
    [TestCase("SUBQ.L #2,D0", "5580")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}