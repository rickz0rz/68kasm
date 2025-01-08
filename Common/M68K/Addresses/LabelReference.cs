namespace Common.M68K.Addresses;

public class LabelReference : BaseAddress
{
    public string LabelName { get; init; }

    public override string ToString()
    {
        return LabelName;
    }
}
