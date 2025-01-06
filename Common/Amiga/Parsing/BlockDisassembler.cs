using System.Text;
using Common.M68K.Instructions;

namespace Common.Amiga.Parsing;

public class BlockDisassembler
{
    public static string Disassemble(Hunk hunk)
    {
        // Todo:
        // - Create a map of all addresses in all sections
        // - Prefill the map with known PC offsets from emulation
        // - Fill the map up slowly with instructions as they're parsed
        // - Fill gaps with DC

        var stringBuilder = new StringBuilder();
        
        var hunkSectionNumber = 0;
        foreach (var hunkSection in hunk.HunkSections)
        {
            var sectionTypeString = hunkSection.SectionType switch
            {
                0x000003E9 => "CODE",
                0x000003EA => "DATA",
                0x000003EB => "BSS",
                _ => "UNKNOWN"
            };
            
            var sectionTypeMemory = hunkSection.SectionMemoryFlag switch
            {
                2 => ",CHIP",
                _ => ""
            };

            stringBuilder.AppendLine($"SECTION_{hunkSectionNumber},{sectionTypeString}{sectionTypeMemory}");
            stringBuilder.AppendLine("; -------------");
            
            // Iterate through the instructions.
            var pc = 0;
            var shouldContinue = true;
            while (pc < hunkSection.Data.Count && shouldContinue)
            {
                Console.WriteLine(BaseInstruction.FromHunk(hunk, hunkSectionNumber, ref pc).ToString());
            }
            
            hunkSectionNumber++;
            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }
}
