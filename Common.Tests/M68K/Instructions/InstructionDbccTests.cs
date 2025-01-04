namespace Common.Tests.M68K.Instructions;

public class InstructionDbccTests
{
    [TestCase("DBF D0,-4", "51C8FFFC")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}