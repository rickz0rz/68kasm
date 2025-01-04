using System.Text;
using Common.M68K.Instructions;

namespace Common.Amiga.Parsing;

public class BlockDisassembler
{
    public static string Disassemble(Hunk hunk)
    {
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
                // stringBuilder.AppendLine(BaseInstruction.FromBytes(hunk, hunkSectionNumber, ref pc).ToString());
            }
            
            hunkSectionNumber++;
            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    private static string ParseAbsoluteAddress(int instruction, HunkSection hunkSection, int pc, List<byte> extraBytesUsed)
    {
        var destRegister = $"A{(instruction >> 9) & 0b111}";

        var addressingMode = (instruction >> 3) & 0b111;
        var addressingModeRegister = (instruction & 0b111);

        var sourceAddress = "";
        int address = 0;
        
        switch (addressingMode)
        {
            case 0b111 when (addressingModeRegister & 0b111) == 0b000:
                // Read 2 bytes in.
                extraBytesUsed.Add(hunkSection.Data[pc + 2]);
                extraBytesUsed.Add(hunkSection.Data[pc + 3]);
                
                // Convert that to an address.
                address = extraBytesUsed[0] * 256 + 
                          extraBytesUsed[1];
                            
                sourceAddress = $"${address:X4}";
                break;
            case 0b111 when (addressingModeRegister & 0b111) == 0b001:
                // Read 4 bytes in
                extraBytesUsed.Add(hunkSection.Data[pc + 2]);
                extraBytesUsed.Add(hunkSection.Data[pc + 3]);
                extraBytesUsed.Add(hunkSection.Data[pc + 4]);
                extraBytesUsed.Add(hunkSection.Data[pc + 5]);
                            
                // Convert that to an address.
                address = extraBytesUsed[0] * 256 * 256 * 256 + 
                          extraBytesUsed[1] * 256 * 256 + 
                          extraBytesUsed[2] * 256 + 
                          extraBytesUsed[3];
                            
                sourceAddress = $"${address:X8}";
                break;
            default:
                throw new NotImplementedException($"Addressing mode: {addressingMode:b3}, addressing mode register: {addressingModeRegister:b3}");
        }
        
        return sourceAddress;
    }
}