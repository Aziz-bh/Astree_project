using System.ComponentModel.DataAnnotations;
using Data.Models;

namespace API.DTOs
{
    public class AutomobileUpdateDto
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Vehicle value must be greater than 0.")]
        public float VehicleValue { get; set; }

        public Guarantees Guarantees { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public string VehicleMake { get; set; }

         public string Model { get; set; }

        public float? TrueVehicleValue { get; set; } // Nullable to indicate it's optional

        // Newly added properties
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Registration number must be alphanumeric.")]
        public string RegistrationNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Engine power must be greater than 0.")]
        public int EnginePower { get; set; }

        [Range(1, 100, ErrorMessage = "Seats number must be between 1 and 100.")]
        public int SeatsNumber { get; set; }
    }
}