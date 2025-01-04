namespace Common.Amiga;

public class HunkSection
{
    public int SectionType { get; set; }
    public List<byte> Data { get; set; }
    public int SectionMemoryFlag { get; set; }
    public Dictionary<int, List<int>> RelocationTables { get; set; }

    public HunkSection()
    {
        Data = new List<byte>();
        RelocationTables = new Dictionary<int, List<int>>();
    }
}