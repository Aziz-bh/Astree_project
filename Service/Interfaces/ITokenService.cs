using Data.Models;

namespace Service.Interfaces
{
    public interface ITokenService
    {
         string CreateToken (User user); 
    }
}