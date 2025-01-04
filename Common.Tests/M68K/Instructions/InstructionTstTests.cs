namespace Common.Tests.M68K.Instructions;

public class InstructionTstTests
{
    [TestCase("TST.L $AC(A3)", "4AAB00AC")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}