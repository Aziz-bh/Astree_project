namespace API.DTOs
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        
        public string CIN { get; set; }
        
        // New properties added
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; }
        public string Civility { get; set; } // Assuming Civil Status will be communicated as a string
        
    }
}
