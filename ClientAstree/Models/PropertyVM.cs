namespace ClientAstree.Models
{
public class PropertyVM
{
    public long Id { get; set; }
    public string ContractType { get; set; } = "0"; // Provide a default type if needed
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public float Quota { get; set; }
    public int UserId { get; set; }
    public string Location { get; set; }
    public string Type { get; set; } // Changed from Enum to String
    public DateTimeOffset YearOfConstruction { get; set; }
    public int PropertyValue { get; set; }
    public ICollection<string> CoveragesList { get; set; } = new List<string>(); // Initialize to avoid null issues
    public string Coverage { get; set; } 
}

    [Flags]
    public enum Guarantees
    {
        None = 0,
        RC = 1 << 0, // 1
        INC = 1 << 1, // 2
        VOL = 1 << 2, // 4
        ASST = 1 << 3, // 8
        TR = 1 << 4, // 16
    }

}