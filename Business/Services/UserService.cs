using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Interfaces;
using Data.Models;
using Data.Persistence;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly AstreeDbContext _context;

        public UserService(
            IMapper mapper,
            UserManager<User> userManager,
            ITokenService tokenService,
            IEmailService emailService,
            AstreeDbContext context
        )
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
            _context = context;
        }

        public async Task<UserDto> RegisterUser(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Email))
                throw new Exception("Email is taken");

            var user = _mapper.Map<User>(registerDto);
            user.UserName = registerDto.Email;
            user.IsDeleted = false;
            user.VerificationToken = CreateRandomToken();
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) throw new Exception("User creation failed");

            var roleResults = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResults.Succeeded) throw new Exception("Adding role failed");

            var verificationLink =
                $"https://localhost:7166/api/Account/verify?token={user.VerificationToken}";
            await _emailService
                .SendEmailAsync(user.Email,
                "Verify your email",
                $"Please verify your account by clicking this link: {verificationLink}");

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                CIN = user.CIN,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender.HasValue ? user.Gender.Value.ToString() : null,
                BirthDate = user.BirthDate,
                Nationality = user.Nationality,
                Civility = user.Civility.HasValue ? user.Civility.Value.ToString() : null,
                Roles = roles.ToList()
            };
        }

        public async Task<UserDto> LoginUser(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email.ToLower());
            if (user == null) throw new Exception("Invalid Email or Password");
            if (user.VerifiedAt == null) throw new Exception("Not verified");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) throw new Exception("Invalid Email or Password");

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                CIN = user.CIN,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender.HasValue ? user.Gender.Value.ToString() : null,
                BirthDate = user.BirthDate,
                Nationality = user.Nationality,
                Civility = user.Civility.HasValue ? user.Civility.Value.ToString() : null,
                Roles = roles.ToList()
            };
        }

        public async Task VerifyUser(string token)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null) throw new Exception("Invalid token");

            user.VerifiedAt = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new Exception("Account verification failed");
        }

        public async Task ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null) return;

            var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            user.PasswordRestToken = resetToken;
            user.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new Exception("Password reset failed");

            var resetLink =
                $"https://localhost:7166/api/Account/reset-password?token={resetToken}&email={user.Email}";
            await _emailService
                .SendEmailAsync(user.Email,
                "Reset Password",
                $"Please reset your password by clicking here: {resetLink}");
        }

        public async Task ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (
                user == null ||
                user.PasswordRestToken != resetPasswordDto.Token ||
                user.ResetTokenExpires < DateTime.UtcNow
            ) throw new Exception("Invalid token");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result.Succeeded) throw new Exception("Could not reset password");

            user.PasswordRestToken = null;
            user.ResetTokenExpires = null;
            await _userManager.UpdateAsync(user);
        }

        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _context.Users
                .Where(u => !(bool)u.IsDeleted)
                .ToListAsync();

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = roles.ToList();
                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<UserDto> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || (bool)user.IsDeleted)
            {
                throw new Exception("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles.ToList();

            return userDto;
        }

        public async Task<UserDto> GetProfile(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || (bool)user.IsDeleted)
            {
                throw new Exception("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles.ToList();

            return userDto;
        }

        public async Task<UserDto> CreateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new Exception("Invalid user data");
            }

            var user = _mapper.Map<User>(userDto);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UpdateUser(int id, UserUpdateDTO userUpdateDTO)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _mapper.Map(userUpdateDTO, user);
            user.UpdatedAt = DateTime.UtcNow;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> UpdateUserProfile(string email, UserUpdateDTO userUpdateDTO)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            _mapper.Map(userUpdateDTO, user);
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to update user");
            }

            return true;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            user.IsDeleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<object>> GetUsersWithRoles()
        {
            var userList = await (
                from user in _context.Users
                orderby user.UserName
                select new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = (from userRole in user.UserRoles
                             join role in _context.Roles on userRole.RoleId equals role.Id
                             select role.Name).ToList()
                }).ToListAsync();

            return userList;
        }

        public async Task<IEnumerable<string>> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new Exception($"{userName} not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames ?? new string[] { };

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded)
            {
                throw new Exception("Failed to add to roles");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
            {
                throw new Exception("Failed to remove the roles");
            }

            return await _userManager.GetRolesAsync(user);
        }

        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(user => user.Id == id);
        }

        private async Task<bool> UserExists(string email)
        {
            return await _userManager.Users.AnyAsync(user => user.Email == email.ToLower());
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }
}
