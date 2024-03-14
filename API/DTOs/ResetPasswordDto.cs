using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
public class ResetPasswordDto
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Token { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    public string NewPassword { get; set; }
}
}