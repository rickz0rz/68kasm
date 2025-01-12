namespace Common.M68K.Addresses;

public class Indirect : BaseAddress
{
    public enum IndirectAddressType
    {
        None,
        PreDecrement,
        PostIncrement
    };

    private string _value;
    private IndirectAddressType _addressType;

    public Indirect(string value, IndirectAddressType addressType) : base()
    {
        _value = value;
        _addressType = addressType;
    }

    public override string ToString()
    {
        return _addressType switch
        {
            IndirectAddressType.None => _value,
            IndirectAddressType.PreDecrement => $"-({_value})",
            IndirectAddressType.PostIncrement => $"({_value})+",
            _ => throw new ArgumentOutOfRangeException(_addressType.ToString())
        };
    }
}
