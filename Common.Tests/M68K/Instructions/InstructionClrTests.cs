namespace Common.Tests.M68K.Instructions;

public class InstructionClrTests
{
    [TestCase("CLR.L -604(A4)", "42ACFDA4")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}