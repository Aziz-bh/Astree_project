using System.Security.Cryptography;
using System.Text;
using API.DTOs;



using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using API.Models;
using AutoMapper;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
           private readonly IMapper _mapper;
 

        public AccountController(IMapper mapper,UserManager<User> userManager, ITokenService tokenService)
        {
            _tokenService = tokenService;
             _mapper = mapper;
            _userManager = userManager;
          
        }



[HttpPost("register")]
public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
{
    if (await UserExists(registerDto.Email))
        return BadRequest("Email is taken");

var user= _mapper.Map<User>(registerDto);
user.UserName=registerDto.Email;
var result=await _userManager.CreateAsync(user ,registerDto.Password);
if(!result.Succeeded) return BadRequest(result.Errors);

return new UserDto
{
    Email = user.Email,
    Token = _tokenService.CreateToken(user),
    FirstName = user.FirstName,
    LastName = user.LastName,
    CIN = user.CIN,
    BirthDate = user.BirthDate,
    Nationality = user.Nationality,
    Civility = user.Civility.ToString(), // Assuming Civility is stored as an enum and needs conversion to string
};
}


[HttpPost("login")]
public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
{
    // Find the user by email
    var user = await _userManager.FindByEmailAsync(loginDto.Email.ToLower());
    
    if (user == null)
        return Unauthorized("Invalid Email or Password");

    // Check the password

    var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

    if (!result)
        return Unauthorized("Invalid Email or Password");

    // Assuming _tokenService.CreateToken(user) is correctly implemented to generate a JWT or similar token
    return new UserDto
    {
        Email = user.Email,
        Token = _tokenService.CreateToken(user),
        FirstName = user.FirstName, // Make sure these properties exist in your User class or are accessible
        LastName = user.LastName,
        CIN = user.CIN,
        BirthDate = user.BirthDate,
        Nationality = user.Nationality,
        Civility = user.Civility.ToString() // Assuming Civility is an enum and ToString() converts it to a string representation
    };
}




                private async Task<bool> UserExists(string email)
        {
            return await _userManager.Users.AnyAsync(user => user.Email == email.ToLower());
        }


    }
}
