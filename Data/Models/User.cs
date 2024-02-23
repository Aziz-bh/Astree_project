using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Models
{
    public class User
    {
        public enum UserGender
        {
            Male,
            Female,
            NA
        }

        public enum UserRole
        {
            Admin,
            Moderator,
            User
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
            Role = UserRole.User;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }

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

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

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

        [JsonConverter(typeof (JsonStringEnumConverter))]
        public UserRole? Role { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Nationality { get; set; }

        [JsonConverter(typeof (JsonStringEnumConverter))]
        public CivilStatus? Civility { get; set; }

        public Boolean? IsDeleted { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
