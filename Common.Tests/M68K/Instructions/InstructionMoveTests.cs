using Common.Amiga;

namespace Common.Tests.M68K.Instructions;

public class InstructionMoveTests
{
    [TestCase("MOVE.L A7,-600(A4)", "294FFDA8")]
    [TestCase("MOVE.L #$00001729,D0", "203C00001729")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}