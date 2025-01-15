namespace Common.Tests.M68K.Instructions;

public class InstructionCmpITests
{
    [TestCase("CMPI.L #$000927C0,D0", "0C80000927C0")]
    [TestCase("CMPI.L #$00000020,$5982(A4)", "0CAC000000205982")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
