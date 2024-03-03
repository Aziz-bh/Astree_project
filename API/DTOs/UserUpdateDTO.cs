using API.Models;

namespace API.DTOs
{
public class UserUpdateDTO
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CIN { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Picture { get; set; }
    public User.UserGender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Nationality { get; set; }
    public User.CivilStatus? Civility { get; set; }
    // Add other properties that can be updated
}

}