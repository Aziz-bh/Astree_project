using System.Drawing;
using System.Drawing.Imaging;
using API.Interfaces;
using API.Models;
using API.Persistence;
using Microsoft.EntityFrameworkCore;
using QRCoder;

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

        public byte[] GeneratePropertyContractQRCode(Property property)
        {
            // Convert property contract data to a string format
            string contractData = $"Id: {property.Id}, Location: {property.Location}, Type: {property.Type}, Value: {property.PropertyValue}, Coverage: {property.Coverage}";

            // Create a QR code generator instance
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(contractData, QRCodeGenerator.ECCLevel.Q);

            // Create a QR code instance from the data
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        qrCodeImage.Save(stream, ImageFormat.Png);
                        return stream.ToArray();
                    }
                }
            }
        }
    }
}