namespace Common.M68K.Addresses;

public class AbsoluteAddress : BaseAddress
{
    public SectionAddress SectionAddress { get; init; }

    public AbsoluteAddress(int sectionNumber, int address)
    {
        SectionAddress = new SectionAddress
        {
            SectionNumber = sectionNumber,
            Address = address
        };
    }

    public AbsoluteAddress(SectionAddress sectionAddress)
    {
     SectionAddress = sectionAddress;
    }

    public override string ToString()
    {
        return SectionAddress.ToString();
    }
}
