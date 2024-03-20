using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using API.Models;
using API.Persistence;
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
        public async Task<IActionResult> GetAllAutomobiles()
        {
            var automobiles = await _automobileService.GetAllAutomobilesAsync();
            return Ok(automobiles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAutomobileById(long id)
        {
            var automobile =
                await _automobileService.GetAutomobileByIdAsync(id);
            if (automobile == null)
            {
                return NotFound();
            }
            return Ok(automobile);
        }

        [HttpPost]
        public async Task<IActionResult>
        CreateAutomobile([FromBody] AutomobileDto automobileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map from DTO to domain model
            var automobile =
                new Automobile {
                    // Example of manual mapping from automobileDto to automobile
                    ContractType = automobileDto.ContractType,
                    StartDate = automobileDto.StartDate,
                    EndDate = automobileDto.EndDate,
                    Quota = automobileDto.Quota,
                    UserId = automobileDto.UserId,
                    VehicleType = automobileDto.VehicleType,
                    RegistrationNumber = automobileDto.RegistrationNumber,
                    RegistrationDate = automobileDto.RegistrationDate,
                    EnginePower = automobileDto.EnginePower,
                    VehicleMake = automobileDto.VehicleMake,
                    SeatsNumber = automobileDto.SeatsNumber,
                    VehicleValue = automobileDto.VehicleValue,
                    TrueVehicleValue = automobileDto.TrueVehicleValue,
                    Guarantees = automobileDto.Guarantees
                };

            try
            {
                var createdAutomobile =
                    await _automobileService.CreateAutomobileAsync(automobile);

                // Assuming manual mapping back to DTO
                var returnDto =
                    new AutomobileDto {
                        // Example of manual mapping from createdAutomobile back to returnDto
                        Id = createdAutomobile.Id,
                        ContractType = createdAutomobile.ContractType,
                        StartDate = createdAutomobile.StartDate,
                        EndDate = createdAutomobile.EndDate,
                        Quota = createdAutomobile.Quota,
                        UserId = createdAutomobile.UserId,
                        VehicleType = createdAutomobile.VehicleType,
                        RegistrationNumber =
                            createdAutomobile.RegistrationNumber,
                        RegistrationDate = createdAutomobile.RegistrationDate,
                        EnginePower = createdAutomobile.EnginePower,
                        VehicleMake = createdAutomobile.VehicleMake,
                        SeatsNumber = createdAutomobile.SeatsNumber,
                        VehicleValue = createdAutomobile.VehicleValue,
                        TrueVehicleValue = createdAutomobile.TrueVehicleValue,
                        Guarantees = createdAutomobile.Guarantees
                    };

                return CreatedAtAction(nameof(GetAutomobileById),
                new { id = createdAutomobile.Id },
                returnDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult>
        UpdateAutomobile(long id, [FromBody] Automobile automobile)
        {
            if (id != automobile.Id)
            {
                return BadRequest();
            }

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
        public async Task<IActionResult> GetAutomobilesByUserId(int userId)
        {
            var automobiles =
                await _automobileService.GetAutomobilesByUserIdAsync(userId);
            if (automobiles == null || !automobiles.Any())
            {
                return NotFound($"No automobiles found for user ID {userId}.");
            }
            return Ok(automobiles);
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
            var contract =
                await _automobileService.GetAutomobilesByUserIdAsync(user.Id);
            if (contract == null || !contract.Any())
            {
                return NotFound($"No contract found for user ID {user.Id}.");
            }

            // Optionally, map the contract to DTOs if you're not sending entities directly
            return Ok(contract);
        }
        // Add other actions as necessary...
    }
}
