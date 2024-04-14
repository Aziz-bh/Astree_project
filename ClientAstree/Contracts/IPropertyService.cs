using ClientAstree.Models;

namespace ClientAstree.Contracts
{
    public interface IPropertyService
    {
        Task<List<PropertyVM>> GetMyPropertyContractsAsync();
                Task<PropertyVM> GetPropertyByIdAsync(long id);
                 Task<List<PropertyVM>> GetUserPropertys(int id);
        Task<PropertyVM> CreatePropertyAsync(PropertyVM property);
        Task UpdatePropertyAsync(PropertyVM property);
        Task DeletePropertyAsync(long id);
    }
}