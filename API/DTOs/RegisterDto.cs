using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The password must be at least 6 characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "The CIN must be exactly 8 characters long.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The CIN must be numbers only.")]
        public string CIN { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Nationality { get; set; }

        [Required]
        public string Civility { get; set; } // Assuming Civil Status as a string

        [Required]
        [RegularExpression(@"^\+?\d{8,9}$", ErrorMessage = "The PhoneNumber field is not a valid phone number.")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Gender { get; set; } // Assuming Gender as a string to keep DTO simple. Adjust as necessary.
    }
}
