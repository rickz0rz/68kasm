namespace Common.M68K.Addresses;

public class GenericString : BaseAddress
{
    private string _value;

    public GenericString(string value) : base()
    {
        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }
}
