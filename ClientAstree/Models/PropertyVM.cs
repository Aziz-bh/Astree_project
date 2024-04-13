namespace ClientAstree.Models
{
    public class PropertyVM
    {
            public long Id { get; set; }
    public string ContractType { get; set; } // Changed from Enum to String
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public float Quota { get; set; }
    public int UserId { get; set; }
    public string Location { get; set; }
    public string Type { get; set; } // Changed from Enum to String
    public DateTimeOffset YearOfConstruction { get; set; }
    public int PropertyValue { get; set; }
    public ICollection<string> CoveragesList { get; set; }
    public string Coverage { get; set; } 
    }
}