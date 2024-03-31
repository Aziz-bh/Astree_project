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

            // Calculate and set the quota before adding the automobile to the context
            automobile = CalculateAndSetQuota(automobile);

            await _context.Automobiles.AddAsync(automobile);
            await _context.SaveChangesAsync();

            return automobile;
        }

        public async Task UpdateAutomobileAsync(Automobile automobile)
        {
            automobile = CalculateAndSetQuota(automobile);
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

        public Automobile CalculateAndSetQuota(Automobile automobile)
        {
            float quota = 0;

            // Calculate RC based on EnginePower
            if (automobile.EnginePower >= 0 && automobile.EnginePower <= 4)
                quota += 500 * 0.7f;
            else if (automobile.EnginePower >= 5 && automobile.EnginePower <= 9)
                quota += 500 * 0.8f;
            else if (
                automobile.EnginePower >= 10 && automobile.EnginePower <= 15
            )
                quota += 500 * 0.9f;
            else if (
                automobile.EnginePower >= 16 && automobile.EnginePower <= 20
            )
                quota += 500;
            else if (
                automobile.EnginePower >= 21 && automobile.EnginePower <= 30
            )
                quota += 500 * 1.1f;
            else if (
                automobile.EnginePower >= 31 && automobile.EnginePower <= 40
            )
                quota += 500 * 1.2f;
            else if (
                automobile.EnginePower >= 41 && automobile.EnginePower <= 50
            )
                quota += 500 * 1.3f;
            else if (
                automobile.EnginePower >= 51 && automobile.EnginePower <= 100
            ) quota += 500 * 1.6f;

            // Calculate INC
            if (automobile.Guarantees.HasFlag(Guarantees.INC))
                quota += automobile.TrueVehicleValue * 0.1f;

            // Calculate VOL based on VehicleValue
            if (automobile.Guarantees.HasFlag(Guarantees.VOL))
            {
                if (
                    automobile.VehicleValue >= 1 &&
                    automobile.VehicleValue <= 10000
                )
                    quota += 50;
                else if (
                    automobile.VehicleValue >= 10001 &&
                    automobile.VehicleValue <= 20000
                )
                    quota += 70;
                else if (
                    automobile.VehicleValue >= 20001 &&
                    automobile.VehicleValue <= 30000
                )
                    quota += 90;
                else if (
                    automobile.VehicleValue >= 30001 &&
                    automobile.VehicleValue <= 50000
                )
                    quota += 130;
                else if (
                    automobile.VehicleValue >= 50001 &&
                    automobile.VehicleValue <= 70000
                )
                    quota += 200;
                else if (automobile.VehicleValue > 70000) quota += 300;
            }

            // Calculate ASST
            if (automobile.Guarantees.HasFlag(Guarantees.ASST)) quota += 50;

            // Calculate TR
            if (automobile.Guarantees.HasFlag(Guarantees.TR))
                quota += automobile.TrueVehicleValue * 0.08f;

            // Update the contract's quota value
            automobile.Quota = quota;
            return automobile;
        }
    }
}
