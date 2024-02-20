using System.Security.Cryptography;
using System.Text;
using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<User>> Register(string email, string password)
        {
            using var hmac = new HMACSHA512();
            var user = new User
            {
                FirstName = email,
                LastName = email,
                Email = email,
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key,
                Role = Models.User.UserRole.User,
                Gender = Models.User.UserGender.NA,
                IsDeleted = false,
                Picture = "https://res.cloudinary.com/dk5b3jxjp/image/upload/v1633660733/astree/placeholder.png",
                nickName=email,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
