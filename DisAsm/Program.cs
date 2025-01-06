using Common.Amiga.Parsing;

namespace DisAsm;

class Program
{
    static void Main(string[] args)
    {
        // Need unit tests to test instruction to bytecode back to instruction
        // Need automated tests to validate a full binary is 1:1
        // General reference: https://web.njit.edu/~rosensta/classes/architecture/252software/code.pdf

        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var binaryPath = Path.Combine(homeDirectory, "Downloads", "Amiga68kTest.bin");
        Console.WriteLine(BlockDisassembler.Disassemble(HunkParser.Parse(binaryPath)));
    }
}
