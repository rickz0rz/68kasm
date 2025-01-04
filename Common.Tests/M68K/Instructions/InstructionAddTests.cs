namespace Common.Tests.M68K.Instructions;

public class InstructionAddTests
{
    [TestCase("ADD.L D1,D0", "D081")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}