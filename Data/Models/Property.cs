namespace Data.Models
{
    public enum PropertyType
    {
        House,
        Apartment,
        Commercial
    }

    [Flags]
    public enum Coverage
    {
        None = 0,
        Fire = 1,
        Theft = 2,
        Natural_Disasters = 4
    }

    public class Property : Contract
    {
        public string Location { get; set; }

        public PropertyType Type { get; set; }

        public DateTime YearOfConstruction { get; set; }

        public int PropertyValue { get; set; }

        public Coverage Coverage { get; set; }
    }
}
