using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Interfaces;
using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<UserDto>> Register( RegisterDto registerDto)
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
            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
        };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
            var user = await _context.Users.SingleOrDefaultAsync(user => user.Email == loginDto.Email);
            if(user == null) return Unauthorized("Invalid Email or Password");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for(int i = 0; i < computedHash.Length; i++){
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Email or Password");
            }
            return new UserDto
            {
                Email = user.Email,
               Token = _tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
        };
        }



                private async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email.ToLower());
        }


    }
}
