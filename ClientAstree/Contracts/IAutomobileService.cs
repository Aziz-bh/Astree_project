using ClientAstree.Models;

namespace ClientAstree.Contracts
{
    public interface IAutomobileService
    {
        Task<List<AutomobileVM>> GetMyAutomobileContractsAsync();
        Task<List<AutomobileVM>> GetUserAutomobiles(int id);
                Task<AutomobileVM> GetAutomobileByIdAsync(long id);
        Task<AutomobileVM> CreateAutomobileAsync(AutomobileVM automobile);
        Task UpdateAutomobileAsync(AutomobileVM automobile);
        Task DeleteAutomobileAsync(long id);
    }
}