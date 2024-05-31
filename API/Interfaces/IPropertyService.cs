using API.Models;

namespace API.Interfaces
{
    public interface IPropertyService
    {
        Task<IEnumerable<Property>> GetAllPropertiesAsync();
        Task<Property> GetPropertyByIdAsync(long id);
        Task<Property> CreatePropertyAsync(Property property);
        Task UpdatePropertyAsync(Property property);
        Task DeletePropertyAsync(long id);
        Task<IEnumerable<Property>> GetPropertiesByUserIdAsync(int userId);
        byte[] GeneratePropertyContractQRCode(string contractUrl);
        Task ValidateContractAsync(long id);
         Task<IEnumerable<Property>> GetAllValidatedPropertiesAsync();
         Task<IEnumerable<Property>> GetAllUnvalidatedPropertiesAsync();
         Task<IEnumerable<Property>> GetUserValidatedPropertiesAsync(int userId);
         Task UnvalidateContractAsync(long id);


    }
}