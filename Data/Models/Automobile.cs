using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
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

    public enum VehicleType { Personal, Business }

    public class Automobile : Contract, IValidatableObject
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
        [StringLength(100)]
        public string Model { get; set; }


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
            if (RegistrationDate > DateTime.UtcNow.Date)
            {
                yield return new ValidationResult("Registration date cannot be in the future.", new[] { nameof(RegistrationDate) });
            }

            // Add any custom validation for guarantees if needed. For flags, specific combination checks may be unnecessary,
            // but you could enforce that at least one guarantee is selected, if that's a requirement.
            if (Guarantees == Guarantees.None)
            {
                yield return new ValidationResult("At least one guarantee must be selected.", new[] { nameof(Guarantees) });
            }
        }
    }
}