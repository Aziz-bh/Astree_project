using API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterUser(RegisterDto registerDto);
        Task<UserDto> LoginUser(LoginDto loginDto);
        Task VerifyUser(string token);
        Task ForgotPassword(ForgotPasswordDto forgotPasswordDto);
        Task ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<IEnumerable<UserDto>> GetUsers();
        Task<UserDto> GetUserById(int id);
        Task<UserDto> GetProfile(string email);
        Task<UserDto> CreateUser(UserDto userDto);
        Task<bool> UpdateUser(int id, UserUpdateDTO userUpdateDTO);
        Task<bool> UpdateUserProfile(string email, UserUpdateDTO userUpdateDTO);
        Task<bool> DeleteUser(int id);
        Task<IEnumerable<object>> GetUsersWithRoles();
        Task<IEnumerable<string>> EditRoles(string userName, RoleEditDto roleEditDto);
    }
}
