namespace Common.Tests.M68K.Instructions;

public class InstructionAddATests
{
    [TestCase("ADDA.L A0,A0", "D1C8")]
    [TestCase("ADDA.L A1,A1", "D3C9")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}