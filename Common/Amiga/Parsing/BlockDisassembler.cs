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
        var instructionMap = new Dictionary<(int hunkSectionNumber, int offset), BaseInstruction>();

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
            var offset = 0;
            var shouldContinue = true;

            // Start the instruction parsing.
            try
            {
                // Only start this on section 0, section 1+ doesn't auto-start @ 0x0
                if (hunkSectionNumber == 0)
                {
                    ParseInstruction(hunk, hunkSectionNumber, 0, instructionMap);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            foreach (var instructionAddress in instructionMap.Keys.Order())
            {
                if (instructionAddress.hunkSectionNumber == hunkSectionNumber)
                {
                    stringBuilder.AppendLine(instructionMap[instructionAddress].ToString());
                }
            }

            hunkSectionNumber++;
            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    private static void ParseInstruction(Hunk hunk, int hunkSectionNumber, int offset,
        Dictionary<(int hunkSectionNumber, int offset), BaseInstruction> instructionMap)
    {
        // If we've previously discovered this offset, abort.
        if (instructionMap.ContainsKey((hunkSectionNumber, offset)))
            return;

        try
        {
            var instruction = BaseInstruction.FromHunk(hunk, hunkSectionNumber, offset);
            instructionMap.Add((hunkSectionNumber, offset), instruction);

            foreach (var nextOffsetAddress in instruction.GetNextOffsetAddresses())
            {
                ParseInstruction(hunk, hunkSectionNumber, nextOffsetAddress, instructionMap);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
