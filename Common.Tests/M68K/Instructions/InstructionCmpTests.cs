namespace Common.Tests.M68K.Instructions;

public class InstructionCmpTests
{
    [TestCase("CMP.B D1,D0", "B001")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
