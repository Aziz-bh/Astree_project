using API.Models;

namespace API.DTOs
{
    public class PropertyDto : ContractDto
    {
        public string Location { get; set; }
        public PropertyType Type { get; set; }
        public DateTime YearOfConstruction { get; set; }
        public int PropertyValue { get; set; }
        public Coverage Coverage { get; set; }
    }
}
