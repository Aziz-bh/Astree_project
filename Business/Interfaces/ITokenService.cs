using API.Models;

namespace Business.Interfaces
{
    public interface ITokenService
    {
         string CreateToken (User user); 
    }
}