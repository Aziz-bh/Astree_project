using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly AstreeDbContext _context;

        public AccountController(AstreeDbContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register( RegisterDto registerDto)
        {
               if (await UserExists(registerDto.Email))
        return BadRequest("Email is taken");
            using var hmac = new HMACSHA512();
            var user = new User
            {
                FirstName = registerDto.Email.ToLower(),
                LastName = registerDto.Email.ToLower(),
                Email = registerDto.Email,
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
                Role = Models.User.UserRole.User,
                Gender = Models.User.UserGender.NA,
                IsDeleted = false,
                Picture = "https://res.cloudinary.com/dk5b3jxjp/image/upload/v1633660733/astree/placeholder.png",
                nickName=registerDto.Email,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
                private async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email.ToLower());
        }


    }
}
