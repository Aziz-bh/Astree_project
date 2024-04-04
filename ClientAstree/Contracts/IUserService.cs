using ClientAstree.Models;

namespace ClientAstree.Contracts
{
    public interface IUserService
    {
        Task<List<UserVM>> GetUsersAsync();

        Task<UserVM> GetUserAsync(int id);
        Task UsersDELETEAsync(int id);
    }
}