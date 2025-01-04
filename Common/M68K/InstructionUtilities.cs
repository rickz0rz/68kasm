using Common.Amiga;

namespace Common.M68K;

public static class InstructionUtilities
{
    public static string ParseSize(int instruction)
    {
        var sizeBits = (instruction >> 12) & 0b11; 
        return sizeBits switch
        {
            0b11 => ".W",
            0b10 => ".L",
            0b01 => ".B",
            _ => $".Unknown_{sizeBits:b2}"
        };
    }
    
    public static int ConvertTwosComplementByte(int value)
    {
        return ((value >> 7) & 1) == 1 ? -1 - (value ^ 0xFF) : value;
    }

    public static int ParseLongWord(Hunk hunk, int hunkSectionId,
        ref int pc, List<byte> extraBytesUsed)
    {
        var returnValue = (hunk.HunkSections[hunkSectionId].Data[pc] << 24) |
                          (hunk.HunkSections[hunkSectionId].Data[pc + 1] << 16) |
                          (hunk.HunkSections[hunkSectionId].Data[pc + 2] << 8) |
                          hunk.HunkSections[hunkSectionId].Data[pc + 3];

        extraBytesUsed.AddRange(hunk.HunkSections[hunkSectionId].Data.Skip(pc).Take(4));
        pc += 4;

        return returnValue;
    }

    public static int ParseWord(Hunk hunk, int hunkSectionId,
        ref int pc, List<byte> extraBytesUsed)
    {
        var returnValue = (hunk.HunkSections[hunkSectionId].Data[pc] << 8) |
                          hunk.HunkSections[hunkSectionId].Data[pc + 1];

        extraBytesUsed.AddRange(hunk.HunkSections[hunkSectionId].Data.Skip(pc).Take(2));
        pc += 2;

        return returnValue;
    }

    public static int ParseByte(Hunk hunk, int hunkSectionId,
        ref int pc, List<byte> extraBytesUsed)
    {
        var returnValue = hunk.HunkSections[hunkSectionId].Data[pc];

        extraBytesUsed.AddRange(hunk.HunkSections[hunkSectionId].Data.Skip(pc).Take(1));
        pc += 1;

        return returnValue;
    }

    public static int ParseTwosComplementWord(Hunk hunk, int hunkSectionId,
        ref int pc, List<byte> extraBytesUsed)
    {
        var value = ParseWord(hunk, hunkSectionId, ref pc, extraBytesUsed);
        return ((value >> 15) & 1) == 1 ? -1 - (value ^ 0xFFFF) : value;
    }

    public static string ParseDestinationAddress(int instruction, Hunk hunk, int hunkSectionId,
        ref int pc, List<byte> extraBytesUsed, int? size = null)
    {
        var register = (instruction >> 9) & 0b111;
        var mode = (instruction >> 6) & 0b111;
        return ParseAddress(hunk, hunkSectionId, ref pc, extraBytesUsed, mode, register, size);
    }

    public static string ParseSourceAddress(int instruction, Hunk hunk, int hunkSectionId,
        ref int pc, List<byte> extraBytesUsed, int? size = null)
    {
        var mode = (instruction >> 3) & 0b111;
        var register = instruction & 0b111;
        return ParseAddress(hunk, hunkSectionId, ref pc, extraBytesUsed, mode, register, size);
    }

    private static string ParseAddress(Hunk hunk, int hunkSectionId,
        ref int pc, List<byte> extraBytesUsed, int mode, int register, int? size)
    {
        return mode switch
        {
            0b000 => $"D{register}",
            0b001 => $"A{register}",
            0b010 => $"(A{register})",
            0b011 => $"(A{register})+",
            0b100 => $"-(A{register})",
            0b101 => $"{FormatValue(ParseTwosComplementWord(hunk, hunkSectionId, ref pc, extraBytesUsed))}(A{register})",
            0b110 => ParseMode110(hunk, hunkSectionId, ref pc, extraBytesUsed, register),
            0b111 when (register & 0b111) == 0b000 =>
                $"${ParseWord(hunk, hunkSectionId, ref pc, extraBytesUsed):X4}",
            0b111 when (register & 0b111) == 0b001 =>
                $"#${ParseLongWord(hunk, hunkSectionId, ref pc, extraBytesUsed):X8}",
            0b111 when (register & 0b111) == 0b100 =>
                ParseMode111_100(hunk, hunkSectionId, ref pc, extraBytesUsed, size),
            0b111 when (register & 0b111) == 0b010 =>
                $"${ParseTwosComplementWord(hunk, hunkSectionId, ref pc, extraBytesUsed):X}(PC)",
            // Missing: 0b111 0b011
            _ => $"Unknown_mode_{mode:b3}_register_{register:b3}"
        };
    }

    private static string ParseMode111_100(Hunk hunk, int hunkSectionId,
        ref int pc, List<byte> extraBytesUsed, int? size)
    {
        if (size is null)
            throw new Exception("Size is required for ParseMode111_001");
        
        var sizeBits = size.Value & 0b11;
        return sizeBits switch
        {
            // For byte operation, grab 16 bits and only use last 8.
            0b01 => FormatValue(ParseWord(hunk, hunkSectionId, ref pc, extraBytesUsed) & 0b11111111),
            0b11 => FormatValue(ParseWord(hunk, hunkSectionId, ref pc, extraBytesUsed)),
            0b10 => FormatValue(ParseLongWord(hunk, hunkSectionId, ref pc, extraBytesUsed)),
            _ => $"Unknown_{sizeBits:b2}"
        };
    }

    private static string ParseMode110(Hunk hunk, int hunkSectionId,
        ref int pc, List<byte> extraBytesUsed, int register)
    {
        var byte1 = ParseByte(hunk, hunkSectionId, ref pc, extraBytesUsed);
        var byte2 = ParseByte(hunk, hunkSectionId, ref pc, extraBytesUsed);

        var size = (byte1 & 0b1000) == 0b1000 ? ".L" : ".W";
        var dRegister = (byte1 >> 4) & 0b111;

        return $"{FormatValue(byte2)}(A{register},D{dRegister}{size})";
    }

    public static string FormatValue(int val, int? precision = null)
    {
        if (val <= 8)
            return val.ToString();
        
        var s = $"{val:X}";
        if (precision.HasValue)
        {
            s = s.PadLeft(precision.Value, '0');
        }
        return $"${s}";
    }
}