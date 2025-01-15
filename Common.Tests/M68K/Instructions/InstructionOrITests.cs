namespace Common.Tests.M68K.Instructions;

public class InstructionOrITests
{
    [TestCase("ORI.W #$0080,D0", "00400080")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
