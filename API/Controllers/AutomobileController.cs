using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using Data.Models;
using Data.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AutomobileController : BaseApiController
    {
        private readonly IAutomobileService _automobileService;
        private readonly UserManager<User> _userManager;
        private readonly AstreeDbContext _context;

        public AutomobileController(
            IAutomobileService automobileService,
            UserManager<User> userManager,
            AstreeDbContext context
        )
        {
            _automobileService = automobileService;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutomobileDto>>> GetAllAutomobiles()
        {
            var automobiles = await _automobileService.GetAllAutomobilesAsync();
            var automobileDtos = automobiles.Select(MapToAutomobileDto).ToList();
            return Ok(automobileDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AutomobileDto>> GetAutomobileById(long id)
        {
            var automobile = await _automobileService.GetAutomobileByIdAsync(id);
            if (automobile == null)
            {
                return NotFound();
            }

            var automobileDto = MapToAutomobileDto(automobile);
            return Ok(automobileDto);
        }

        [HttpPost]
        public async Task<ActionResult<AutomobileDto>> CreateAutomobile([FromBody] AutomobileDto automobileDto)
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

            var automobile = MapToAutomobile(automobileDto, user.Id);

            try
            {
                var createdAutomobile = await _automobileService.CreateAutomobileAsync(automobile);
                var returnDto = MapToAutomobileDto(createdAutomobile);
                return CreatedAtAction(nameof(GetAutomobileById), new { id = createdAutomobile.Id }, returnDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAutomobile(long id, [FromBody] AutomobileUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var automobile = await _automobileService.GetAutomobileByIdAsync(id);
            if (automobile == null)
            {
                return NotFound();
            }

            MapUpdateDtoToAutomobile(automobile, updateDto);

            await _automobileService.UpdateAutomobileAsync(automobile);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAutomobile(long id)
        {
            await _automobileService.DeleteAutomobileAsync(id);
            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AutomobileDto>>> GetAutomobilesByUserId(int userId)
        {
            var automobiles = await _automobileService.GetAutomobilesByUserIdAsync(userId);
            if (automobiles == null || !automobiles.Any())
            {
                return NotFound($"No automobiles found for user ID {userId}.");
            }

            var automobileDtos = automobiles.Select(MapToAutomobileDto).ToList();
            return Ok(automobileDtos);
        }

        [HttpGet("mycontracts")]
        public async Task<ActionResult<IEnumerable<AutomobileDto>>> MyContracts()
        {
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var automobiles = await _automobileService.GetAutomobilesByUserIdAsync(user.Id);
            if (automobiles == null || !automobiles.Any())
            {
                return NotFound($"No contracts found for user ID {user.Id}.");
            }

            var automobileDtos = automobiles.Select(MapToAutomobileDto).ToList();
            return Ok(automobileDtos);
        }

        [HttpGet("{id}/qr")]
        public async Task<IActionResult> GetContractQrCode(long id)
        {
            var automobile = await _automobileService.GetAutomobileByIdAsync(id);
            if (automobile == null)
            {
                return NotFound();
            }

            var contractUrl = $"https://localhost:7054/AutomobileContract/ContractDetails/{id}";
            var qrCodeBytes = _automobileService.GenerateContractQRCode(contractUrl);
            return File(qrCodeBytes, "image/png");
        }

        [HttpPost("{id}/validate")]
        public async Task<IActionResult> ValidateAutomobileContract(long id)
        {
            try
            {
                await _automobileService.ValidateContractAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("validated")]
        public async Task<ActionResult<IEnumerable<AutomobileDto>>> GetAllValidatedAutomobiles()
        {
            var automobiles = await _automobileService.GetAllValidatedAutomobilesAsync();
            var automobileDtos = automobiles.Select(MapToAutomobileDto).ToList();
            return Ok(automobileDtos);
        }

        [HttpGet("unvalidated")]
        public async Task<ActionResult<IEnumerable<AutomobileDto>>> GetAllUnvalidatedAutomobiles()
        {
            var automobiles = await _automobileService.GetAllUnvalidatedAutomobilesAsync();
            var automobileDtos = automobiles.Select(MapToAutomobileDto).ToList();
            return Ok(automobileDtos);
        }

        [HttpGet("mycontracts/validated")]
        public async Task<ActionResult<IEnumerable<AutomobileDto>>> GetUserValidatedAutomobiles()
        {
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var automobiles = await _automobileService.GetUserValidatedAutomobilesAsync(user.Id);
            var automobileDtos = automobiles.Select(MapToAutomobileDto).ToList();
            return Ok(automobileDtos);
        }

        [HttpPost("{id}/unvalidate")]
        public async Task<IActionResult> UnvalidateAutomobileContract(long id)
        {
            try
            {
                await _automobileService.UnvalidateContractAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private AutomobileDto MapToAutomobileDto(Automobile automobile)
        {
            return new AutomobileDto
            {
                Id = automobile.Id,
                ContractType = automobile.ContractType,
                StartDate = automobile.StartDate,
                EndDate = automobile.EndDate,
                Quota = automobile.Quota,
                UserId = automobile.UserId,
                VehicleType = automobile.VehicleType,
                RegistrationNumber = automobile.RegistrationNumber,
                RegistrationDate = automobile.RegistrationDate,
                EnginePower = automobile.EnginePower,
                VehicleMake = automobile.VehicleMake,
                SeatsNumber = automobile.SeatsNumber,
                VehicleValue = automobile.VehicleValue,
                TrueVehicleValue = automobile.TrueVehicleValue,
                Model = automobile.Model,
                Guarantees = automobile.Guarantees
            };
        }

        private Automobile MapToAutomobile(AutomobileDto dto, int userId)
        {
            return new Automobile
            {
                ContractType = dto.ContractType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Quota = dto.Quota,
                UserId = userId,
                VehicleType = dto.VehicleType,
                RegistrationNumber = dto.RegistrationNumber,
                RegistrationDate = dto.RegistrationDate,
                EnginePower = dto.EnginePower,
                VehicleMake = dto.VehicleMake,
                SeatsNumber = dto.SeatsNumber,
                VehicleValue = dto.VehicleValue,
                TrueVehicleValue = dto.TrueVehicleValue,
                Model = dto.Model,
                Guarantees = dto.Guarantees
            };
        }

        private void MapUpdateDtoToAutomobile(Automobile automobile, AutomobileUpdateDto updateDto)
        {
            automobile.StartDate = updateDto.StartDate;
            automobile.EndDate = updateDto.EndDate;
            automobile.VehicleValue = updateDto.VehicleValue;
            automobile.Guarantees = updateDto.Guarantees;
            automobile.VehicleMake = updateDto.VehicleMake;
            automobile.Model = updateDto.Model;

            if (updateDto.TrueVehicleValue.HasValue)
            {
                automobile.TrueVehicleValue = updateDto.TrueVehicleValue.Value;
            }

            automobile.RegistrationNumber = updateDto.RegistrationNumber;
            automobile.RegistrationDate = updateDto.RegistrationDate;
            automobile.EnginePower = updateDto.EnginePower;
            automobile.SeatsNumber = updateDto.SeatsNumber;
        }
    }
}
