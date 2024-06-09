using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Interfaces;
using Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;

        private readonly ITokenService _tokenService;

        private readonly IMapper _mapper;

        private readonly IEmailService _emailService;

        public AccountController(
            IMapper mapper,
            UserManager<User> userManager,
            ITokenService tokenService,
            IEmailService emailService
        )
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>>
        Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Email))
                return BadRequest("Email is taken");

            var user = _mapper.Map<User>(registerDto);
            user.UserName = registerDto.Email;
            user.IsDeleted = false;
            user.VerificationToken = CreateRandomToken();
            var result =
                await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResults = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResults.Succeeded) return BadRequest(roleResults.Errors);

            var verificationLink =
                $"https://localhost:7166/api/Account/verify?token={user.VerificationToken}";
            await _emailService
                .SendEmailAsync(user.Email,
                "Verify your email",
                $"Please verify your account by clicking this link: {verificationLink}");

            // Fetch roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto {
                Id = user.Id,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                CIN = user.CIN,
                PhoneNumber = user.PhoneNumber,
                Gender =
                    user.Gender.HasValue ? user.Gender.Value.ToString() : null,
                BirthDate = user.BirthDate,
                Nationality = user.Nationality,
                Civility =
                    user.Civility.HasValue
                        ? user.Civility.Value.ToString()
                        : null,
                Roles = roles.ToList() // Assign the roles here
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Find the user by email
            var user =
                await _userManager.FindByEmailAsync(loginDto.Email.ToLower());

            if (user == null) return Unauthorized("Invalid Email or Password");
            if (user.VerifiedAt == null) return Unauthorized("Not verified");

            // Check the password
            var result =
                await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result) return Unauthorized("Invalid Email or Password");

            // Fetch roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto {
                Id = user.Id,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                CIN = user.CIN,
                PhoneNumber = user.PhoneNumber,
                Gender =
                    user.Gender.HasValue ? user.Gender.Value.ToString() : null,
                BirthDate = user.BirthDate,
                Nationality = user.Nationality,
                Civility =
                    user.Civility.HasValue
                        ? user.Civility.Value.ToString()
                        : null,
                Roles = roles.ToList() // Assign the roles here
            };
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify([FromQuery] string token)
        {
            var user =
                await _userManager
                    .Users
                    .FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null)
            {
                return BadRequest("Invalid token");
            }

            user.VerifiedAt = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("An error occurred while verifying the account.");
            }

            return Ok("Account verified successfully");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult>
        ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user =
                await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return Ok("If an account with this email exists, a password reset link has been sent.");
            }

           var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));



            user.PasswordRestToken = resetToken;
            user.ResetTokenExpires = DateTime.UtcNow.AddDays(1); // 1 day expiry time

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("An error occurred while processing your request.");
            }

            // Send the email
            var resetLink =
                $"https://localhost:7166/api/Account/reset-password?token={resetToken}&email={user.Email}";
            await _emailService
                .SendEmailAsync(user.Email,
                "Reset Password",
                $"Please reset your password by clicking here: {resetLink}");

            return Ok("If an account with this email exists, a password reset link has been sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult>
        ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user =
                await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (
                user == null ||
                user.PasswordRestToken != resetPasswordDto.Token ||
                user.ResetTokenExpires < DateTime.UtcNow
            )
            {
                return BadRequest("Invalid token.");
            }

            var result =
                await _userManager
                    .ResetPasswordAsync(user,
                    resetPasswordDto.Token,
                    resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest("Could not reset password.");
            }

 
            user.PasswordRestToken = null;
            user.ResetTokenExpires = null;
            await _userManager.UpdateAsync(user);

            return Ok("Password has been reset successfully.");
        }

        private async Task<bool> UserExists(string email)
        {
            return await _userManager
                .Users
                .AnyAsync(user => user.Email == email.ToLower());
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }
}
