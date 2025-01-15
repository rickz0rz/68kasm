namespace Common.Tests.M68K.Instructions;

public class InstructionOrTests
{
    [TestCase("OR.L D0,$57DC(A4)", "81AC57DC")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
