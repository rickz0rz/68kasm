﻿using Common.Amiga;

namespace Common.Tests.M68K.Instructions;

public class InstructionMoveMTests
{
    [TestCase("MOVEM.L D1-D6/A0-A6,-(A7)", "48E77EFE")]
    [TestCase("MOVEM.L (A7)+,D1-D6/A0-A6", "4CDF7F7E")]
    public void RunFullTest(string assembly, string bytesString)
    {
        TestHelpers.RunFullTest(assembly, bytesString);
    }
}
