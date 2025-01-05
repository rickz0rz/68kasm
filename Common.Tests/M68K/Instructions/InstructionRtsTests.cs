namespace Common.Tests.M68K.Instructions;

public class InstructionRtsTests
{
    [TestCase("RTS", "4E75")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
