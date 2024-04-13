using ClientAstree.Models;

namespace ClientAstree.Contracts
{
    public interface IPropertyService
    {
        Task<List<PropertyVM>> GetMyPropertyContractsAsync();
    }
}