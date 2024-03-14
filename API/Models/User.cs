using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class User: IdentityUser<int>
    {
        public enum UserGender
        {
            Male,
            Female,
            NA
        }


        public enum CivilStatus
        {
            Single,
            Married,
            Divorced,
            Widowed
        }

        public User()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }


        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Required]
        [
            StringLength(
                8,
                MinimumLength = 8,
                ErrorMessage = "The CIN must be exactly 8 characters long.")
        ]
        [
            RegularExpression(
                "^[0-9]*$",
                ErrorMessage = "The CIN must be numbers only.")
        ]
        public string? CIN { get; set; }



        [
            RegularExpression(
                @"^\d{8,9}$",
                ErrorMessage =
                    "The PhoneNumber field is not a valid phone number.")
        ]
        public string? PhoneNumber { get; set; }

        public string? Picture { get; set; }

        [JsonConverter(typeof (JsonStringEnumConverter))]
        public UserGender? Gender { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Nationality { get; set; }

        [JsonConverter(typeof (JsonStringEnumConverter))]
        public CivilStatus? Civility { get; set; }

        public string? VerificationToken  { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public string?  PasswordRestToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        public Boolean? IsDeleted { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }
        public virtual ICollection<Complaint> Complaints { get; set; }
    }
}
