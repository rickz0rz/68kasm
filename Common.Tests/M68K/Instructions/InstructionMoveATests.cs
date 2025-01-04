using Common.Amiga;

namespace Common.Tests.M68K.Instructions;

public class InstructionMoveATests
{
    [TestCase("MOVEA.L A0,A2", "2448")]
    [TestCase("MOVEA.L $0004,A6", "2C780004")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}