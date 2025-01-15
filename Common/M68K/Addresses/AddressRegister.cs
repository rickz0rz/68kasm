namespace Common.M68K.Addresses;

public class AddressRegister : BaseAddress
{
    public int RegisterNumber { get; set; }

    public AddressRegister(int registerNumber)
    {
        RegisterNumber = registerNumber;
    }

    public override string ToString()
    {
        return $"A{RegisterNumber}";
    }
}
