using API.Interfaces;
using API.Models;
using API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly AstreeDbContext _context;

        public PropertyService(AstreeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            return await _context.Properties.ToListAsync();
        }

        public async Task<Property> GetPropertyByIdAsync(long id)
        {
            return await _context.Properties.FindAsync(id);
        }

public async Task<Property> CreatePropertyAsync(Property property)
{
    if (property == null)
    {
        throw new ArgumentNullException(nameof(property));
    }

    var today = DateTime.UtcNow.Date; // Use UtcNow to standardize the time zone.

    // Ensure StartDate is today or in the future.
    if (property.StartDate.Date < today)
    {
        throw new ArgumentException("StartDate must be today or in the future.");
    }

    // Ensure EndDate is after StartDate.
    if (property.EndDate <= property.StartDate)
    {
        throw new ArgumentException("EndDate must be after StartDate.");
    }

    await _context.Properties.AddAsync(property);
    await _context.SaveChangesAsync();

    return property;
}

        public async Task UpdatePropertyAsync(Property property)
        {
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePropertyAsync(long id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
            }
        }

         public async Task<IEnumerable<Property>> GetPropertiesByUserIdAsync(int userId)
        {
            return await _context.Properties
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }
    }
}