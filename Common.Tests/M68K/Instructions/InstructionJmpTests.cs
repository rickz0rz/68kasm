namespace Common.Tests.M68K.Instructions;

public class InstructionJmpTests
{
    [TestCase("JMP #$000318D4", "4EF9000318D4")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
