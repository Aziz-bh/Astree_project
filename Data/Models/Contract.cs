using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public enum ContractType
    {
        Automobile,
        Property
    }

    public abstract class Contract:IValidatableObject
    {
        public long Id { get; set; }

        [Required]
        public ContractType ContractType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public float Quota { get; set; }
        
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public bool Validated { get; set; } 

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate < DateTime.UtcNow.Date)
            {
                yield return new ValidationResult(
                    "StartDate must be today or in the future.",
                    new[] { nameof(StartDate) });
            }

            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "EndDate must be after StartDate.",
                    new[] { nameof(EndDate) });
            }
        }
    }
}
