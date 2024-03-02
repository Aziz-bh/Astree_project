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
user.IsDeleted=false;
var result=await _userManager.CreateAsync(user ,registerDto.Password);
if(!result.Succeeded) return BadRequest(result.Errors);

        var roleResults= await _userManager.AddToRoleAsync(user, "Member");
        if(!roleResults.Succeeded) return BadRequest(roleResults.Errors); 

return new UserDto
{
    Email = user.Email,
    Token = await _tokenService.CreateToken(user),
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



  
    return new UserDto
    {
        Email = user.Email,
        Token = await _tokenService.CreateToken(user),
        FirstName = user.FirstName, 
        LastName = user.LastName,
        CIN = user.CIN,
        BirthDate = user.BirthDate,
        Nationality = user.Nationality,
        Civility = user.Civility.ToString() 
    };
}




                private async Task<bool> UserExists(string email)
        {
            return await _userManager.Users.AnyAsync(user => user.Email == email.ToLower());
        }


    }
}
