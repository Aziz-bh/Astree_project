using System.ComponentModel.DataAnnotations;

namespace ClientAstree.Models;

public class CreateUser
{
    [Required]
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; } 
    public string? CIN { get; set; } 
    public DateTime? BirthDate { get; set; } 
    public string? Nationality { get; set; } 
    public string? Gender { get; set; } 
    public string? Civility { get; set; } 
}

public class UserVM : CreateUser
{
    public int? Id { get; set; }
}
