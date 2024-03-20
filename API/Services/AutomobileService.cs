using System.Collections.Generic;
using System.Threading.Tasks;
using API.Interfaces;
using API.Models;
using API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class AutomobileService : IAutomobileService
    {
        private readonly AstreeDbContext _context;

        public AutomobileService(AstreeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Automobile>> GetAllAutomobilesAsync()
        {
            return await _context.Automobiles.ToListAsync();
        }

        public async Task<Automobile> GetAutomobileByIdAsync(long id)
        {
            return await _context.Automobiles.FindAsync(id);
        }

        public async Task<Automobile>
        CreateAutomobileAsync(Automobile automobile)
        {
            if (automobile == null)
            {
                throw new ArgumentNullException(nameof(automobile));
            }

            var today = DateTime.UtcNow.Date; // Assuming dates are in UTC for consistency.

            // Check if StartDate is today or in the future.
            if (automobile.StartDate.Date < today)
            {
                throw new ArgumentException("StartDate must be today or in the future.");
            }

            // Check if EndDate is after StartDate.
            if (automobile.EndDate <= automobile.StartDate)
            {
                throw new ArgumentException("EndDate must be after StartDate.");
            }

            await _context.Automobiles.AddAsync(automobile);
            await _context.SaveChangesAsync();

            return automobile;
        }

        public async Task UpdateAutomobileAsync(Automobile automobile)
        {
            _context.Automobiles.Update (automobile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAutomobileAsync(long id)
        {
            var automobile = await _context.Automobiles.FindAsync(id);
            if (automobile != null)
            {
                _context.Automobiles.Remove (automobile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Automobile>>
        GetAutomobilesByUserIdAsync(int userId)
        {
            return await _context
                .Automobiles
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }
    }
}
