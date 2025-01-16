namespace Common.Tests.M68K.Instructions;

public class InstructionUnlkTests
{
    [TestCase("UNLK A5", "4E5D")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
