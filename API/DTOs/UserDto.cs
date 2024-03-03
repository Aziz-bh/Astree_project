using System;
using System.Collections.Generic;

namespace API.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Picture { get; set; }
        public string Gender { get; set; } 
        public List<string> Roles { get; set; } = new List<string>();
        public string CIN { get; set; }
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; }
        public string Civility { get; set; } 
    }
}
