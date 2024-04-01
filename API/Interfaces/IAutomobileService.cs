using API.Models;

namespace API.Interfaces
{
   public interface IAutomobileService
    {
        Task<IEnumerable<Automobile>> GetAllAutomobilesAsync();
        Task<Automobile> GetAutomobileByIdAsync(long id);
        Task<Automobile> CreateAutomobileAsync(Automobile automobile);
        Task UpdateAutomobileAsync(Automobile automobile);
        Task DeleteAutomobileAsync(long id);
        Task<IEnumerable<Automobile>> GetAutomobilesByUserIdAsync(int userId);
        byte[] GenerateContractQRCode(Automobile automobile);
    }
}