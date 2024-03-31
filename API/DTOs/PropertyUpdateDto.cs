using System.ComponentModel.DataAnnotations;
using API.Models;

namespace API.DTOs
{
    public class PropertyUpdateDto
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string Location { get; set; }

        public PropertyType Type { get; set; }

        [DataType(DataType.Date)]
        public DateTime YearOfConstruction { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Property value must be greater than 0.")]
        public int PropertyValue { get; set; }

        public Coverage Coverage { get; set; }

    }
}