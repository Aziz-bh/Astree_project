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
                            Model = auto.Model ,
                            Guarantees = auto.Guarantees // This sets the integer value (not shown in output if not desired)
                            // GuaranteesList is dynamically generated based on the Guarantees property
                        })
                    .ToList();

            return Ok(automobileDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AutomobileDto>> GetAutomobileById(long id)
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
                    Model=automobile.Model,
                    Guarantees = automobile.Guarantees // This sets the integer value (not directly shown if you choose to hide it in your DTO definition)
                    // GuaranteesList is dynamically generated based on the Guarantees property
                };

            return Ok(automobileDto);
        }

        [HttpPost]
        public async Task<ActionResult<AutomobileDto>>
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
                    Model=automobileDto.Model,
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
                        Model=createdAutomobile.Model,
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
public async Task<IActionResult> UpdateAutomobile(long id, [FromBody] AutomobileUpdateDto updateDto)
{


    if (!ModelState.IsValid)
    {
        Console.WriteLine("Model state is invalid");
        return BadRequest(ModelState);
    }
    Console.WriteLine("Model state is valid");

    var automobile = await _automobileService.GetAutomobileByIdAsync(id);
    Console.WriteLine(automobile == null ? "Automobile not found" : $"Automobile found with id: {automobile.Id}");

    if (automobile == null)
    {
        return NotFound();
    }

    // Map the updated fields from the DTO to the automobile entity
    automobile.StartDate = updateDto.StartDate;
    Console.WriteLine($"Updated StartDate: {automobile.StartDate}");

    automobile.EndDate = updateDto.EndDate;
    Console.WriteLine($"Updated EndDate: {automobile.EndDate}");

    automobile.VehicleValue = updateDto.VehicleValue;
    Console.WriteLine($"Updated VehicleValue: {automobile.VehicleValue}");

    automobile.Guarantees = updateDto.Guarantees;
    Console.WriteLine($"Updated Guarantees: {automobile.Guarantees}");

    automobile.VehicleMake = updateDto.VehicleMake;
    Console.WriteLine($"Updated VehicleMake: {automobile.VehicleMake}");

    automobile.Model = updateDto.Model;
    Console.WriteLine($"Updated Model: {automobile.Model}");

    if (updateDto.TrueVehicleValue.HasValue)
    {
        automobile.TrueVehicleValue = updateDto.TrueVehicleValue.Value;
        Console.WriteLine($"Updated TrueVehicleValue: {automobile.TrueVehicleValue}");
    }

    // Mapping the newly added fields
    automobile.RegistrationNumber = updateDto.RegistrationNumber;
    Console.WriteLine($"Updated RegistrationNumber: {automobile.RegistrationNumber}");

    automobile.RegistrationDate = updateDto.RegistrationDate;
    Console.WriteLine($"Updated RegistrationDate: {automobile.RegistrationDate}");

    automobile.EnginePower = updateDto.EnginePower;
    Console.WriteLine($"Updated EnginePower: {automobile.EnginePower}");

    automobile.SeatsNumber = updateDto.SeatsNumber;
    Console.WriteLine($"Updated SeatsNumber: {automobile.SeatsNumber}");

    // Perform the update operation
    Console.WriteLine("Updating automobile...");
    await _automobileService.UpdateAutomobileAsync(automobile);
    Console.WriteLine("Automobile updated");

    return NoContent(); // or return appropriate response
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
            var automobiles =
                await _automobileService.GetAutomobilesByUserIdAsync(userId);
            if (automobiles == null || !automobiles.Any())
            {
                return NotFound($"No automobiles found for user ID {userId}.");
            }
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
                            Guarantees = automobile.Guarantees,
                        Model = automobile.Model

                            // Ensure to map any additional fields if present
                        })
                    .ToList();

            return Ok(automobileDtos);
       
        }

        [HttpGet("mycontracts")]
        public async Task<ActionResult<IEnumerable<AutomobileDto>>> mycontracts()
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
                            Guarantees = automobile.Guarantees,
                            Model = automobile.Model

                            // Ensure to map any additional fields if present
                        })
                    .ToList();

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
    var automobileDtos = automobiles.Select(auto => new AutomobileDto
    {
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
        Model = auto.Model,
        Guarantees = auto.Guarantees
    }).ToList();

    return Ok(automobileDtos);
}

[HttpGet("unvalidated")]
public async Task<ActionResult<IEnumerable<AutomobileDto>>> GetAllUnvalidatedAutomobiles()
{
    var automobiles = await _automobileService.GetAllUnvalidatedAutomobilesAsync();
    var automobileDtos = automobiles.Select(auto => new AutomobileDto
    {
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
        Model = auto.Model,
        Guarantees = auto.Guarantees
    }).ToList();

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
    var automobileDtos = automobiles.Select(auto => new AutomobileDto
    {
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
        Model = auto.Model,
        Guarantees = auto.Guarantees
    }).ToList();

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

    }
}
