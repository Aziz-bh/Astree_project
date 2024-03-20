using System.ComponentModel.DataAnnotations;

namespace API.Models
{
        [Flags] // This attribute indicates that the enum can be treated as a bit field; that is, a set of flags.
    public enum Guarantees
    {
        None = 0,
        CollisionCoverage = 1,
        ComprehensiveCoverage = 2,
        Uninsured = 4
    }
    public enum VehicleType { Personal, Business }

public class Automobile : Contract,IValidatableObject
{
        [Required]
        public VehicleType VehicleType { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Registration number must be alphanumeric.")]
        public string RegistrationNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Engine power must be greater than 0.")]
        public int EnginePower { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string VehicleMake { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Seats number must be between 1 and 100.")]
        public int SeatsNumber { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Vehicle value must be greater than 0.")]
        public float VehicleValue { get; set; }

        public float TrueVehicleValue { get; set; } // Consider if any validation is needed

        [Required]
        public Guarantees Guarantees { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Include all validations from the base Contract class
            foreach (var validationResult in base.Validate(validationContext))
            {
                yield return validationResult;
            }

            // Custom validation logic for Automobile-specific properties
            // Example: Ensuring RegistrationDate is not in the future
            if (RegistrationDate > DateTime.UtcNow.Date)
            {
                yield return new ValidationResult(
                    "Registration date cannot be in the future.",
                    new[] { nameof(RegistrationDate) });
            }

            // Example: Validate Guarantees contains a valid combination if necessary
            // This is more complex due to the flags attribute and might not be needed
            // if you allow all combinations.
        }
    }
}