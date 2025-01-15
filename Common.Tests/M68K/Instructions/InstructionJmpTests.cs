namespace Common.Tests.M68K.Instructions;

public class InstructionJmpTests
{
    // This normally wouldn't have [0] and would just be: JMP #$000318D4
    // but i'm doing this to reference the section number.
    [TestCase("JMP #$[0]000318D4", "4EF9000318D4")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
