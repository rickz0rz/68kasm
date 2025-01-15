using System.Text;
using Common.M68K;
using Common.M68K.Instructions;

namespace Common.Amiga.Parsing;

public class BlockDisassembler
{
    public static DisassemblerOptions Options { get; } = new();

    public static string Disassemble(Hunk hunk)
    {
        // Todo:
        // - Create a map of all addresses in all sections
        // - Prefill the map with known PC offsets from emulation
        // - Fill the map up slowly with instructions as they're parsed
        // - Fill gaps with DC

        var stringBuilder = new StringBuilder();

        var hunkSectionNumber = 0;
        var instructionMap = new Dictionary<SectionAddress, BaseInstruction>();

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

            // Start the instruction parsing, iterating through each one
            try
            {
                // Only start this on section 0, section 1+ doesn't auto-start @ 0x0
                if (hunkSectionNumber == 0)
                {
                    ParseInstruction(hunk, new SectionAddress { SectionNumber = hunkSectionNumber, Address = 0 }, instructionMap);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            foreach (var instructionAddress in instructionMap.Keys.OrderBy(t => t.Address))
            {
                if (instructionAddress.SectionNumber == hunkSectionNumber)
                {
                    if (hunk.Labels.TryGetValue(instructionAddress, out var label))
                    {
                        stringBuilder.AppendLine($"{label}:");
                    }

                    var instruction = instructionMap[instructionAddress];

                    // Maybe can do something where if it's less than 1, make the comment inline.
                    if (instruction.Comments.Count > 1)
                    {
                        foreach (var comment in instruction.Comments)
                        {
                            stringBuilder.AppendLine($"; {comment}");
                        }
                    }

                    stringBuilder.Append(instruction);

                    if (instruction.Comments.Count == 1)
                    {
                        stringBuilder.Append($" ;; {instruction.Comments.First()}");
                    }

                    stringBuilder.AppendLine();
                }
            }

            hunkSectionNumber++;
            stringBuilder.AppendLine();
        }

        if (Options.DebugPrint)
            Console.WriteLine();

        return stringBuilder.ToString();
    }

    private static void ParseInstruction(Hunk hunk, SectionAddress sectionAddress,
        Dictionary<SectionAddress, BaseInstruction> instructionMap)
    {
        // If we've previously discovered this offset, abort.
        if (instructionMap.ContainsKey(sectionAddress))
            return;

        try
        {
            var instruction = BaseInstruction.FromHunk(hunk, sectionAddress);
            instructionMap.Add(sectionAddress, instruction);

            foreach (var nextOffsetAddress in instruction.GetNextOffsetAddresses())
            {
                ParseInstruction(hunk, nextOffsetAddress, instructionMap);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
