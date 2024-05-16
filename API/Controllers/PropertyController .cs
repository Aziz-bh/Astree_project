using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using API.Models;
using API.Persistence;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PropertyController : BaseApiController
    {
        private readonly IPropertyService _propertyService;

        private readonly UserManager<User> _userManager;

        private readonly AstreeDbContext _context;

        public PropertyController(
            IPropertyService propertyService,
            UserManager<User> userManager,
            AstreeDbContext context
        )
        {
            _propertyService = propertyService;
            _userManager = userManager;
            _context = context;
        }

        // Example: Get all properties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAllProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDto>> GetPropertyById(long id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var propertyDto =
                new PropertyDto {
                    // Assign all other necessary fields from the property to the DTO
                        Id = property.Id,
                        ContractType = property.ContractType,
                        StartDate = property.StartDate,
                        EndDate = property.EndDate,
                        Quota = property.Quota,
                        Location = property.Location,
                        Type = property.Type,
                        YearOfConstruction = property.YearOfConstruction,
                        PropertyValue = property.PropertyValue,
                        Coverage = property.Coverage,
                        UserId = property.UserId // This will be used to generate CoveragesList
                };

            return Ok(propertyDto);
        }

        // Example: Create a new property
        [HttpPost]
        public async Task<ActionResult<PropertyDto>> CreateProperty([FromBody] PropertyDto propertyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user email from JWT token
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch user by email
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Business rule validations
            if (propertyDto.EndDate <= propertyDto.StartDate)
            {
                return BadRequest("End date must be later than start date.");
            }

            if (propertyDto.PropertyValue <= 0)
            {
                return BadRequest("Property value must be a positive number.");
            }

            // Map from DTO to domain model
            var property = new Property
            {
                UserId = user.Id,
                StartDate = propertyDto.StartDate,
                EndDate = propertyDto.EndDate,
                Quota = 0,
                Location = propertyDto.Location,
                Type = propertyDto.Type,
                YearOfConstruction = propertyDto.YearOfConstruction,
                PropertyValue = propertyDto.PropertyValue,
                Coverage = propertyDto.Coverage
            };

            try
            {
                var createdProperty = await _propertyService.CreatePropertyAsync(property);

                var returnDto = new PropertyDto
                {
                    Id = createdProperty.Id,
                    ContractType = createdProperty.ContractType,
                    StartDate = createdProperty.StartDate,
                    EndDate = createdProperty.EndDate,
                    Quota = createdProperty.Quota,
                    Location = createdProperty.Location,
                    Type = createdProperty.Type,
                    YearOfConstruction = createdProperty.YearOfConstruction,
                    PropertyValue = createdProperty.PropertyValue,
                    Coverage = createdProperty.Coverage,
                    UserId = createdProperty.UserId
                };

                return CreatedAtAction(nameof(GetPropertyById), new { id = createdProperty.Id }, returnDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(long id, [FromBody] PropertyUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            // Business rule validations
            if (updateDto.EndDate <= updateDto.StartDate)
            {
                return BadRequest("End date must be later than start date.");
            }

            if (updateDto.PropertyValue <= 0)
            {
                return BadRequest("Property value must be a positive number.");
            }

            // Map the updated fields from the DTO to the property entity
            property.StartDate = updateDto.StartDate;
            property.EndDate = updateDto.EndDate;
            property.Location = updateDto.Location;
            property.Type = updateDto.Type;
            property.YearOfConstruction = updateDto.YearOfConstruction;
            property.PropertyValue = updateDto.PropertyValue;
            property.Coverage = updateDto.Coverage;

            // Perform the update operation
            await _propertyService.UpdatePropertyAsync(property);

            return NoContent(); // or return appropriate response
        }

        // Example: Delete a property
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(long id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            await _propertyService.DeletePropertyAsync(id);
            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetPropertiesByUserId(int userId)
        {
            var properties =
                await _propertyService.GetPropertiesByUserIdAsync(userId);
            if (properties == null || !properties.Any())
            {
                return NotFound($"No properties found for user ID {userId}.");
            }
 
                                var propertyDtos =
                properties
                    .Select(property =>
                        new PropertyDto {
                        // Manual mapping from createdProperty to returnDto
                        Id = property.Id,
                        ContractType = property.ContractType,
                        StartDate = property.StartDate,
                        EndDate = property.EndDate,
                        Quota = property.Quota,
                        Location = property.Location,
                        Type = property.Type,
                        YearOfConstruction = property.YearOfConstruction,
                        PropertyValue = property.PropertyValue,
                        Coverage = property.Coverage,
                        UserId = property.UserId // Make sure PropertyDto has a UserId property.
                    })
                    .ToList();
            // Optionally, map the properties to DTOs if you're not sending entities directly
            return Ok(propertyDtos);
        }

        [HttpGet("mycontracts")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> mycontracts()
        {
            var userEmail =
                HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var properties =
                await _propertyService.GetPropertiesByUserIdAsync(user.Id);
            if (properties == null || !properties.Any())
            {
                return NotFound($"No properties found for user ID {user.Id}.");
            }

            // Map properties to PropertyDto list
            var propertyDtos =
                properties
                    .Select(property =>
                        new PropertyDto {
                            // Assign all other necessary fields from the property to the DTO
                              Id = property.Id,
                        ContractType = property.ContractType,
                        StartDate = property.StartDate,
                        EndDate = property.EndDate,
                        Quota = property.Quota,
                        Location = property.Location,
                        Type = property.Type,
                        YearOfConstruction = property.YearOfConstruction,
                        PropertyValue = property.PropertyValue,
                        Coverage = property.Coverage,
                        UserId = property.UserId 
                            // Make sure to include any additional fields that PropertyDto expects
                            // and are available in the Property entity
                        })
                    .ToList();

            return Ok(propertyDtos);
        }

        // Add other actions as necessary...
        [HttpGet("{id}/qr")]
        public async Task<IActionResult> GetPropertyContractQrCode(long id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var qrCodeBytes =
                _propertyService.GeneratePropertyContractQRCode(property);
            return File(qrCodeBytes, "image/png");
        }
    }
}
