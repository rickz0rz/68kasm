namespace Common.M68K.Instructions;

public class InstructionDc
{
    // This is a weird one.
    // Do this until we have a \0?
    // https://68k.hax.com/DC
    // It looks like constant data is always aligned to 4 bytes. So, realistically:
    // For strings, just check to see if data is ascii and ends with 3 or less \0 bytes?
}
