namespace Common.Tests.M68K.Instructions;

public class InstructionLeaTests
{
    [TestCase("LEA $00008000,A4", "49F900008000")]
    [TestCase("LEA $00007D68,A3", "47F900007D68")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}