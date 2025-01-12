namespace Common.M68K.Addresses;

public class Immediate : BaseAddress
{
    private string _value;

    public Immediate(string value) : base()
    {
        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }
}
