using ClientAstree.Models;

namespace ClientAstree.Contracts
{
    public interface IAutomobileService
    {
        Task<List<AutomobileVM>> GetMyAutomobileContractsAsync();
    }
}