namespace Common.M68K.Addresses;

public class Register : BaseAddress
{
    public string RegisterName { get; init; }

    public override string ToString()
    {
        return RegisterName;
    }
}
