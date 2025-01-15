namespace Common.Tests.M68K.Instructions;

public class InstructionCmpITests
{
    [TestCase("CMPI.L #$000927C0,D0", "0C80000927C0")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
