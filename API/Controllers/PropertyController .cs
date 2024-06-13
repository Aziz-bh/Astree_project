using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using Data.Models;
using Data.Persistence;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAllProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            var propertyDtos = properties.Select(MapToPropertyDto).ToList();
            return Ok(propertyDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDto>> GetPropertyById(long id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var propertyDto = MapToPropertyDto(property);
            return Ok(propertyDto);
        }

        [HttpPost]
        public async Task<ActionResult<PropertyDto>> CreateProperty([FromBody] PropertyDto propertyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            if (propertyDto.EndDate <= propertyDto.StartDate)
            {
                return BadRequest("End date must be later than start date.");
            }

            if (propertyDto.PropertyValue <= 0)
            {
                return BadRequest("Property value must be a positive number.");
            }

            var property = MapToProperty(propertyDto, user.Id);

            try
            {
                var createdProperty = await _propertyService.CreatePropertyAsync(property);
                var returnDto = MapToPropertyDto(createdProperty);
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

            if (updateDto.EndDate <= updateDto.StartDate)
            {
                return BadRequest("End date must be later than start date.");
            }

            if (updateDto.PropertyValue <= 0)
            {
                return BadRequest("Property value must be a positive number.");
            }

            MapUpdateDtoToProperty(property, updateDto);

            await _propertyService.UpdatePropertyAsync(property);

            return NoContent();
        }

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
            var properties = await _propertyService.GetPropertiesByUserIdAsync(userId);
            if (properties == null || !properties.Any())
            {
                return NotFound($"No properties found for user ID {userId}.");
            }

            var propertyDtos = properties.Select(MapToPropertyDto).ToList();
            return Ok(propertyDtos);
        }

        [HttpGet("mycontracts")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> MyContracts()
        {
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
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

            var propertyDtos = properties.Select(MapToPropertyDto).ToList();
            return Ok(propertyDtos);
        }

        [HttpGet("{id}/qr")]
        public async Task<IActionResult> GetPropertyContractQrCode(long id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var contractUrl = $"https://localhost:7054/PropertyContract/Details/{id}";
            var qrCodeBytes = _propertyService.GeneratePropertyContractQRCode(contractUrl);
            return File(qrCodeBytes, "image/png");
        }

        [HttpPost("{id}/validate")]
        public async Task<IActionResult> ValidatePropertyContract(long id)
        {
            try
            {
                await _propertyService.ValidateContractAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("validated")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAllValidatedProperties()
        {
            var properties = await _propertyService.GetAllValidatedPropertiesAsync();
            var propertyDtos = properties.Select(MapToPropertyDto).ToList();
            return Ok(propertyDtos);
        }

        [HttpGet("unvalidated")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAllUnvalidatedProperties()
        {
            var properties = await _propertyService.GetAllUnvalidatedPropertiesAsync();
            var propertyDtos = properties.Select(MapToPropertyDto).ToList();
            return Ok(propertyDtos);
        }

        [HttpGet("mycontracts/validated")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetUserValidatedProperties()
        {
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var properties = await _propertyService.GetUserValidatedPropertiesAsync(user.Id);
            var propertyDtos = properties.Select(MapToPropertyDto).ToList();
            return Ok(propertyDtos);
        }

        [HttpPost("{id}/unvalidate")]
        public async Task<IActionResult> UnvalidatePropertyContract(long id)
        {
            try
            {
                await _propertyService.UnvalidateContractAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private PropertyDto MapToPropertyDto(Property property)
        {
            return new PropertyDto
            {
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
            };
        }

        private Property MapToProperty(PropertyDto dto, int userId)
        {
            return new Property
            {
                UserId = userId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Quota = dto.Quota,
                Location = dto.Location,
                Type = dto.Type,
                YearOfConstruction = dto.YearOfConstruction,
                PropertyValue = dto.PropertyValue,
                Coverage = dto.Coverage
            };
        }

        private void MapUpdateDtoToProperty(Property property, PropertyUpdateDto updateDto)
        {
            property.StartDate = updateDto.StartDate;
            property.EndDate = updateDto.EndDate;
            property.Location = updateDto.Location;
            property.Type = updateDto.Type;
            property.YearOfConstruction = updateDto.YearOfConstruction;
            property.PropertyValue = updateDto.PropertyValue;
            property.Coverage = updateDto.Coverage;
        }
    }
}
