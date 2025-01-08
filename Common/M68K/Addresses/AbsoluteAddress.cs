namespace Common.M68K.Addresses;

public class AbsoluteAddress : BaseAddress
{
    public SectionAddress SectionAddress { get; init; }

    public override string ToString()
    {
        return SectionAddress.ToString();
    }
}
