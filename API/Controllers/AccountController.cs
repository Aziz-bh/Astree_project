using System.Security.Cryptography;
using System.Text;
using API.DTOs;

using Data.Models;
using Data.Persistence;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Business.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly AstreeDbContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(AstreeDbContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }



[HttpPost("register")]
public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
{
    if (await UserExists(registerDto.Email))
        return BadRequest("Email is taken");

    using var hmac = new HMACSHA512();

    var user = new User
    {
        FirstName = registerDto.FirstName,
        LastName = registerDto.LastName,
        Email = registerDto.Email.ToLower(),
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        PasswordSalt = hmac.Key,
        PhoneNumber = registerDto.PhoneNumber,
        // Gender = Enum.Parse<Data.Models.User.UserGender>(registerDto.Gender, true), // Assuming Gender is valid enum string
        Gender = Data.Models.User.UserGender.NA, 
        BirthDate = registerDto.BirthDate,
        Nationality = registerDto.Nationality,
        Civility = Data.Models.User.CivilStatus.Married, // Convert Civility string to enum
        CIN = registerDto.CIN,
        Role = Data.Models.User.UserRole.User, // Default role for new registrations
        IsDeleted = false,
        Picture = "https://res.cloudinary.com/dk5b3jxjp/image/upload/v1633660733/astree/placeholder.png" // Default picture
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

return new UserDto
{
    Email = user.Email,
    Token = _tokenService.CreateToken(user),
    FirstName = user.FirstName,
    LastName = user.LastName,
    Role = user.Role.ToString(),
    CIN = user.CIN,
    BirthDate = user.BirthDate,
    Nationality = user.Nationality,
    Civility = user.Civility.ToString(), // Assuming Civility is stored as an enum and needs conversion to string
};
}


[HttpPost("login")]
public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
{
    var user = await _context.Users
        .SingleOrDefaultAsync(user => user.Email == loginDto.Email.ToLower());

    if (user == null) 
        return Unauthorized("Invalid Email or Password");

    using var hmac = new HMACSHA512(user.PasswordSalt);
    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

    for (int i = 0; i < computedHash.Length; i++)
    {
        if (computedHash[i] != user.PasswordHash[i]) 
            return Unauthorized("Invalid Email or Password");
    }

    return new UserDto
    {
        Email = user.Email,
        Token = _tokenService.CreateToken(user),
        FirstName = user.FirstName,
        LastName = user.LastName,
        Role = user.Role.ToString(),
        CIN = user.CIN,
        BirthDate = user.BirthDate,
        Nationality = user.Nationality,
        Civility = user.Civility.ToString() // Assuming Civility is stored as an enum and needs conversion to string
    };
}



                private async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email.ToLower());
        }


    }
}
