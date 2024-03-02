using API.Models;

namespace Business.Interfaces
{
    public interface ITokenService
    {
         Task<string> CreateToken (User user); 
    }
}