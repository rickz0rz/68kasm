namespace Common.Tests.M68K.Instructions;

public class InstructionAndITests
{
    [TestCase("ANDI.W #$FFFE,D0", "0240FFFE")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}