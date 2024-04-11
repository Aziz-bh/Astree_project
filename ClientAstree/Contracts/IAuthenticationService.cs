using ClientAstree.Models;
using ClientAstree.Services.Base;

namespace ClientAstree.Contracts
{
    public interface IAuthenticationService
    {
  Task<bool> Authenticate(string email, string password);
        // Task<bool> Register(RegisterVM registration);
        Task<bool> Register(RegisterDto registration);
        Task Logout();
    }
}