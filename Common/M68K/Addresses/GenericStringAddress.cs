namespace Common.M68K.Addresses;

public class GenericStringAddress : BaseAddress
{
    private string _value;

    public GenericStringAddress(string value) : base()
    {
        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }
}
