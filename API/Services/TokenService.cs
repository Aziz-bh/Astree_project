using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
// using Business.Interfaces;
using Data.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using API.Interfaces;
using Microsoft.AspNetCore.Identity; // Add this line

namespace API.Services
{
  public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<User> _userManager;
        public TokenService(IConfiguration config,UserManager<User> userManager)
        {
            _userManager = userManager;
            _key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public async Task<string> CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email)
                // new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                // new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                // new Claim(ClaimTypes.Role, user.Role)
            };
             var roles = await _userManager.GetRolesAsync(user);
    var firstRole = roles.FirstOrDefault();
    if (firstRole != null) // Ensure there is at least one role
    {
        claims.Add(new Claim(ClaimTypes.Role, firstRole));
    }
            var creds=new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
