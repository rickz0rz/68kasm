namespace Common.Tests.M68K.Instructions;

public class InstructionAddITests
{
    [TestCase("ADDI.L #$00000080,D0", "068000000080")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}