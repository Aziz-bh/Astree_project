namespace API.DTOs
{
    public class UpdatePasswordDto
    {
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}