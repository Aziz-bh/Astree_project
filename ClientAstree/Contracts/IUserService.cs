using ClientAstree.Models;
using ClientAstree.Services.Base;

namespace ClientAstree.Contracts
{
    public interface IUserService
    {
        Task<List<UserVM>> GetUsersAsync();

        Task<UserVM> GetUserAsync(int id);
        Task UsersDELETEAsync(int id);
        Task<UserVM> ProfileAsync();

        Task UpdateAsync(UserUpdateDTO body);
    }
}