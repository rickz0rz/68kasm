namespace Common.Tests.M68K.Instructions;

public class InstructionPeaTests
{
    [TestCase("PEA -664(A4)", "486CFD68")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}