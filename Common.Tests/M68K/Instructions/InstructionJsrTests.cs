namespace Common.Tests.M68K.Instructions;

public class InstructionJsrTests
{
    [TestCase("JSR -306(A6)", "4EAEFECE")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}