namespace Common.Tests.M68K.Instructions;

public class InstructionNopTests
{
    [TestCase("NOP", "4E71")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}