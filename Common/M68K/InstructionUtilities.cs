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

    // This is going to be fun. Replace this and all downstream methods
    // to return BaseAddress
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
            /* Note: On amiga m68k:
               d0-d1 : scratch data registers
               d2-d7 : non-volatile data registers
               a0-a1 : scratch address registers
               a2-a4 : non-volatile address registers
               a5 : frame pointer on Amiga, non-volatile address register otherwise
               a6 : frame pointer on other systems, non-volatile address register on Amiga
               a7 : stack pointer
               See: https://wiki.freepascal.org/m68k
            */

            // "A7 is located at the end of an area allocated via AllocMem. Available space depends on stack setting on the shell. Type "stack" command (with no parameter) to query actual value.
            // https://eab.abime.net/showpost.php?p=1323812&postcount=3
            // https://en.wikipedia.org/wiki/Motorola_68000_series#Architecture

            // https://mrjester.hapisan.com/04_MC68/Sect01Part04/Index.html
            // "They can be used in much the same way as data registers, except, you cannot perform byte instructions on them
            //   (only word or long-word), and some instructions that work on data registers, will not work on address registers,
            //   but we’ll get into those details later on.

            // https://www.pagetable.com/docs/amigados_tripos/amigados_manual.pdf
            // 2.2.2 Calling Resident Libraries
            // You should note that there are two ways of calling system routines from a user
            // assembly program. C programmers simply call the function as specified. You
            // usually call a system routine in assembler by placing the library base pointer
            // for that resident library in register A6 and then jumping to a suitable negative
            // offset from that pointer. The offsets are available to you as absolute externals in
            // the Amiga library, with names of the form __LVOname. So, for instance, a call
            // could be JSR __LVOname(A6), where you have loaded A6 with a suitable library
            // base pointer. These base pointers are available to you from the Openlibrary
            // call to Exec; you can find the base pointer for Exec at location 4 (the only
            // absolute location used in the Amiga). This location is also known as AbsExecBase
            // which is defined in Amiga.lib. (See the ROM Kernel Manual for further details
            // on Exec.)
            // You can call certain RAM-based resident libraries and the AmigaDOS library
            // in this way, if required. Note that the AmigaDOS library is called "dos.library".
            // However, you do not need to use A6 to hold a pointer to the library base; you
            // may use any other register if you need to. In addition, you may call AmigaDOS
            // using the resident library call feature of the linker. In this case, simply code a
            // JSR to the entry point and the linker notes the fact that you have used a
            // reference to a resident library. When your code is loaded into memory, the
            // loader automatically opens the library and closes it for you when you have
            // unloaded. The loader automatically patches references to AmigaDOS entry
            // points to refer to the correct offset from the library base pointer.

            // immediate
            0b000 => $"D{register}", // set d register to specific value
            0b001 => $"A{register}", // same for a

            // pointer (only a(ddress) registers)
            0b010 => $"(A{register})", // "element pointed at" i.e. register holds address pointer
            0b011 => $"(A{register})+", // "element pointed at", post-increment after doing operation
            0b100 => $"-(A{register})", // pre-decrment address in register, "element pointed at"

            0b101 => $"{FormatValue(ParseTwosComplementWord(hunk, hunkSectionId, ref pc, extraBytesUsed))}(A{register})",
            0b110 => ParseMode110(hunk, hunkSectionId, ref pc, extraBytesUsed, register),
            0b111 when (register & 0b111) == 0b000 =>
                $"${ParseWord(hunk, hunkSectionId, ref pc, extraBytesUsed):X4}",
            0b111 when (register & 0b111) == 0b001 =>
                $"#${ParseLongWord(hunk, hunkSectionId, ref pc, extraBytesUsed):X8}",
            0b111 when (register & 0b111) == 0b100 =>
                Parse_Mode111_Register100(hunk, hunkSectionId, ref pc, extraBytesUsed, size),
            0b111 when (register & 0b111) == 0b010 =>
                $"${ParseTwosComplementWord(hunk, hunkSectionId, ref pc, extraBytesUsed):X}(PC)",
            // Missing: 0b111 0b011
            _ => $"Unknown_mode_{mode:b3}_register_{register:b3}"
        };
    }

    private static string Parse_Mode111_Register100(Hunk hunk, int hunkSectionId,
        ref int pc, List<byte> extraBytesUsed, int? size)
    {
        if (size is null)
            throw new Exception("Size is required for Parsing Mode 111/Register 001");
        
        var sizeBits = size.Value & 0b11;
        return "#" + sizeBits switch
        {
            // For byte operation, grab 16 bits and only use last 8.
            0b01 => FormatValue(ParseWord(hunk, hunkSectionId, ref pc, extraBytesUsed) & 0b11111111, 2),
            0b11 => FormatValue(ParseWord(hunk, hunkSectionId, ref pc, extraBytesUsed), 4),
            0b10 => FormatValue(ParseLongWord(hunk, hunkSectionId, ref pc, extraBytesUsed), 8),
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
