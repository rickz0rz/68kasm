namespace Common.Tests.M68K.Instructions;

public class InstructionLinkTests
{
    [TestCase("LINK.W A5,#-16", "4E55FFF0")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
