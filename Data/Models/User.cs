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

        public User()
        {
            Role = UserRole.User;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? nickName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        [
            RegularExpression(
                @"^\d{10,15}$",
                ErrorMessage =
                    "The PhoneNumber field is not a valid phone number.")
        ]
        public string? PhoneNumber { get; set; }

        public string Picture { get; set; }

        [JsonConverter(typeof (JsonStringEnumConverter))]
        public UserGender? Gender { get; set; }

        [JsonConverter(typeof (JsonStringEnumConverter))]
        public UserRole? Role { get; set; }

        public Boolean? IsDeleted { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
