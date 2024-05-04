using System.ComponentModel.DataAnnotations;

namespace ClientAstree.Models
{
    public class RegisterVM
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, RegularExpression(@"^\d{8}$")]
        public string Cin { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Nationality { get; set; }

          public string ConcatenatedNationality { get; set; }

        [Required]
        public string Civility { get; set; }

        [Required]
        public string Gender { get; set; }
    }
    }
