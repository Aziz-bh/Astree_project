using ClientAstree.Models;

namespace ClientAstree.Contracts
{
    public interface IPropertyService
    {
        Task<List<PropertyVM>> GetMyPropertyContractsAsync();

        Task<List<PropertyVM>> PropertyAllAsync();
                Task<PropertyVM> GetPropertyByIdAsync(long id);
                 Task<List<PropertyVM>> GetUserPropertys(int id);
        Task<PropertyVM> CreatePropertyAsync(PropertyVM property);
        Task UpdatePropertyAsync(PropertyVM property);
        Task DeletePropertyAsync(long id);
        Task<List<PropertyVM>> GetAllValidatedPropertiesAsync(); // New method
        Task<List<PropertyVM>> GetAllUnvalidatedPropertiesAsync(); // New method
        Task<List<PropertyVM>> GetUserValidatedPropertiesAsync(); 
        Task Validate2Async(long id);
        Task Unvalidate2Async(long id);
    }
}