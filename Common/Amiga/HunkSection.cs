namespace Common.Amiga;

public class HunkSection
{
    public int SectionType { get; set; }
    public List<byte> Data { get; set; }
    public int SectionMemoryFlag { get; set; }
    public Dictionary<int, List<int>> RelocationTables { get; }

    public HunkSection()
    {
        Data = [];
        RelocationTables = new Dictionary<int, List<int>>();
    }

    public HunkSection(Dictionary<int, List<int>> relocationTables)
    {
        RelocationTables = relocationTables;
    }
}
