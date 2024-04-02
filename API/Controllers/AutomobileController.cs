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

            // Convert each Automobile entity to AutomobileDto
            var automobileDtos =
                automobiles
                    .Select(auto =>
                        new AutomobileDto {
                            Id = auto.Id,
                            ContractType = auto.ContractType,
                            StartDate = auto.StartDate,
                            EndDate = auto.EndDate,
                            Quota = auto.Quota,
                            UserId = auto.UserId,
                            VehicleType = auto.VehicleType,
                            RegistrationNumber = auto.RegistrationNumber,
                            RegistrationDate = auto.RegistrationDate,
                            EnginePower = auto.EnginePower,
                            VehicleMake = auto.VehicleMake,
                            SeatsNumber = auto.SeatsNumber,
                            VehicleValue = auto.VehicleValue,
                            TrueVehicleValue = auto.TrueVehicleValue,
                            Guarantees = auto.Guarantees // This sets the integer value (not shown in output if not desired)
                            // GuaranteesList is dynamically generated based on the Guarantees property
                        })
                    .ToList();

            return Ok(automobileDtos);
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

            // Convert the Automobile entity to AutomobileDto
            var automobileDto =
                new AutomobileDto {
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
                    Guarantees = automobile.Guarantees // This sets the integer value (not directly shown if you choose to hide it in your DTO definition)
                    // GuaranteesList is dynamically generated based on the Guarantees property
                };

            return Ok(automobileDto);
        }

        [HttpPost]
        public async Task<IActionResult>
        CreateAutomobile([FromBody] AutomobileDto automobileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user email from JWT token
            var userEmail =
                HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch user by email
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Map from DTO to domain model
            var automobile =
                new Automobile {
                    // Example of manual mapping from automobileDto to automobile
                    ContractType = automobileDto.ContractType,
                    StartDate = automobileDto.StartDate,
                    EndDate = automobileDto.EndDate,
                    Quota = automobileDto.Quota,
                    UserId = user.Id,
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
        UpdateAutomobile(long id, [FromBody] AutomobileUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var automobile =
                await _automobileService.GetAutomobileByIdAsync(id);
            if (automobile == null)
            {
                return NotFound();
            }

            // Map the updated fields from the DTO to the automobile entity
            automobile.StartDate = updateDto.StartDate;
            automobile.EndDate = updateDto.EndDate;
            automobile.VehicleValue = updateDto.VehicleValue;
            automobile.Guarantees = updateDto.Guarantees;
            automobile.VehicleMake = updateDto.VehicleMake;
            automobile.Model = updateDto.Model;
            if (
                updateDto.TrueVehicleValue.HasValue // Check if provided before updating
            )
            {
                automobile.TrueVehicleValue = updateDto.TrueVehicleValue.Value;
            }

            // Mapping the newly added fields
            automobile.RegistrationNumber = updateDto.RegistrationNumber;
            automobile.RegistrationDate = updateDto.RegistrationDate;
            automobile.EnginePower = updateDto.EnginePower;
            automobile.SeatsNumber = updateDto.SeatsNumber;

            // Perform the update operation
            await _automobileService.UpdateAutomobileAsync(automobile);

            return NoContent(); // or return appropriate response
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

            var automobiles =
                await _automobileService.GetAutomobilesByUserIdAsync(user.Id);
            if (automobiles == null || !automobiles.Any())
            {
                return NotFound($"No contracts found for user ID {user.Id}.");
            }

            // Map each Automobile entity to AutomobileDto
            var automobileDtos =
                automobiles
                    .Select(automobile =>
                        new AutomobileDto {
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
                            Guarantees = automobile.Guarantees
                            // Ensure to map any additional fields if present
                        })
                    .ToList();

            return Ok(automobileDtos);
        }

        [HttpGet("{id}/qr")]
        public async Task<IActionResult> GetContractQrCode(long id)
        {
            var automobile =
                await _automobileService.GetAutomobileByIdAsync(id);
            if (automobile == null)
            {
                return NotFound();
            }

            var qrCodeBytes =
                _automobileService.GenerateContractQRCode(automobile);
            return File(qrCodeBytes, "image/png");
        }
        // Add other actions as necessary...
    }
}
