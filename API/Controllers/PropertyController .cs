using API.DTOs;
using API.Interfaces;
using API.Models;
using API.Persistence;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class PropertyController : BaseApiController
    {
        private readonly IPropertyService _propertyService;
        private readonly UserManager<User> _userManager;
        private readonly AstreeDbContext _context;

        public PropertyController(IPropertyService propertyService, UserManager<User> userManager, AstreeDbContext context)
        {
            _propertyService = propertyService;
            _userManager = userManager;
            _context = context;
        }

        // Example: Get all properties
        [HttpGet]
        public async Task<IActionResult> GetAllProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        // Example: Get a single property by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyById(long id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return Ok(property);
        }

        // Example: Create a new property
[HttpPost]
public async Task<IActionResult> CreateProperty([FromBody] PropertyDto propertyDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var property = new Property
    {
        // Manual mapping from propertyDto to property
        UserId = propertyDto.UserId,
        ContractType = propertyDto.ContractType,
        StartDate = propertyDto.StartDate,
        EndDate = propertyDto.EndDate,
        Quota = propertyDto.Quota,
        Location = propertyDto.Location,
        Type = propertyDto.Type,
        YearOfConstruction = propertyDto.YearOfConstruction,
        PropertyValue = propertyDto.PropertyValue,
        Coverage = propertyDto.Coverage,
    };

    try
    {
        var createdProperty = await _propertyService.CreatePropertyAsync(property);

        var returnDto = new PropertyDto
        {
            // Manual mapping from createdProperty to returnDto
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
            UserId = createdProperty.UserId // Make sure PropertyDto has a UserId property.
        };

        return CreatedAtAction(nameof(GetPropertyById), new { id = createdProperty.Id }, returnDto);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ex.Message);
    }
}



        // Example: Update an existing property
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(long id, [FromBody] Property property)
        {
            if (id != property.Id)
            {
                return BadRequest();
            }

            try
            {
                await _propertyService.UpdatePropertyAsync(property);
            }
            catch
            {
                // Handle the case where the property wasn't found
                return NotFound();
            }

            return NoContent();
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
        public async Task<IActionResult> GetPropertiesByUserId(int userId)
        {
            var properties = await _propertyService.GetPropertiesByUserIdAsync(userId);
            if (properties == null || !properties.Any())
            {
                return NotFound($"No properties found for user ID {userId}.");
            }

            // Optionally, map the properties to DTOs if you're not sending entities directly
            return Ok(properties);
        }

        
        [HttpGet("mycontracts")]
        public async Task<IActionResult> mycontracts()
        {

                        var userEmail =
                HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }
            var properties = await _propertyService.GetPropertiesByUserIdAsync(user.Id);
            if (properties == null || !properties.Any())
            {
                return NotFound($"No properties found for user ID {user.Id}.");
            }

            // Optionally, map the properties to DTOs if you're not sending entities directly
            return Ok(properties);
        }
        // Add other actions as necessary...
    }
}
