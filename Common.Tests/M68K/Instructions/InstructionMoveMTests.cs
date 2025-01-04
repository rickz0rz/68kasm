using Common.Amiga;

namespace Common.Tests.M68K.Instructions;

public class InstructionMoveMTests
{
    [TestCase("MOVEM.L D1-D6/A0-A6,-(A7)", "48E77EFE")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}