namespace Common.M68K.Addresses;

public class DataRegister : BaseAddress
{
    public int RegisterNumber { get; set; }

    public DataRegister(int registerNumber)
    {
        RegisterNumber = registerNumber;
    }

    public override string ToString()
    {
        return $"D{RegisterNumber}";
    }
}
