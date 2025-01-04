namespace Common.Tests.M68K.Instructions;

public class InstructionMoveQTests
{
    [TestCase("MOVEQ #0,D1", "7200")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}