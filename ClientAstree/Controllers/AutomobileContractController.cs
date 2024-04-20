using ClientAstree.Contracts;
using ClientAstree.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientAstree.Controllers
{
    public class AutomobileContractController: Controller
    {
                private readonly IUserService _userService;
          private readonly IAuthenticationService _authService;
            private readonly IAutomobileService _automobileService;
            private readonly IPropertyService _propertyService;

        public AutomobileContractController(IUserService leaveTypeService,IAuthenticationService authService,IAutomobileService automobileService,IPropertyService propertyService)
        {
            this._userService = leaveTypeService;
            this._authService = authService;
            this._automobileService=automobileService;
            this._propertyService=propertyService;
        }

[HttpGet]
public async Task<IActionResult> Index()
{
    var contracts = await _automobileService.GetMyAutomobileContractsAsync();
    return View(contracts);
}


        [HttpGet]
        public IActionResult Create()
        {
            return View(new AutomobileVM());
        }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(AutomobileVM model, IFormCollection form)
{
    // Manual extraction as a fallback or to ensure correct binding
    model.VehicleMake = model.VehicleMake ?? form["VehicleMake"];
    model.Model = model.Model ?? form["Model"];
    model.VehicleType = form["VehicleType"]; // Ensure conversion if necessary
    model.RegistrationNumber = model.RegistrationNumber ?? form["RegistrationNumber"];
    model.ContractType = model.ContractType ?? form["ContractType"];

    // Parse individual fields that may require conversion
    if (DateTimeOffset.TryParse(form["RegistrationDate"], out var registrationDate)) {
        model.RegistrationDate = registrationDate;
    }
    if (DateTimeOffset.TryParse(form["StartDate"], out var startDate)) {
        model.StartDate = startDate;
    }
    if (DateTimeOffset.TryParse(form["EndDate"], out var endDate)) {
        model.EndDate = endDate;
    }

    // Parse numeric fields ensuring correct type handling
    model.EnginePower = int.TryParse(form["EnginePower"], out var enginePower) ? enginePower : 0;
    model.SeatsNumber = int.TryParse(form["SeatsNumber"], out var seatsNumber) ? seatsNumber : 0;
    model.VehicleValue = float.TryParse(form["VehicleValue"], out var vehicleValue) ? vehicleValue : 0;
    model.TrueVehicleValue = float.TryParse(form["TrueVehicleValue"], out var trueVehicleValue) ? trueVehicleValue : 0;

    // Handling multiple checkbox selections for guarantees
    var guarantees = form["Guarantees"].Select(int.Parse).ToArray();
    model.Guarantees = guarantees.Sum().ToString(); // Sum of values to handle flags

        ModelState.ClearValidationState("VehicleMake");
    ModelState.ClearValidationState("Model");
    ModelState.ClearValidationState("VehicleType");
    ModelState.ClearValidationState("RegistrationNumber");
    // Clear other states as necessary

    // Revalidate model after manual assignment
    TryValidateModel(model);

    // Log the complete model state to console for debugging
    // Console.WriteLine($"Received Model: VehicleMake={model.VehicleMake}, Model={model.Model}, VehicleType={model.VehicleType}, " +
    //     $"RegistrationNumber={model.RegistrationNumber}, RegistrationDate={model.RegistrationDate}, StartDate={model.StartDate}, " +
    //     $"EndDate={model.EndDate}, EnginePower={model.EnginePower}, SeatsNumber={model.SeatsNumber}, " +
    //     $"VehicleValue={model.VehicleValue}, TrueVehicleValue={model.TrueVehicleValue}, Guarantees={model.Guarantees}");


        var createdAutomobile = await _automobileService.CreateAutomobileAsync(model);
        if (createdAutomobile != null)
        {
            return RedirectToAction(nameof(Index));
        }
        ModelState.AddModelError("", "Failed to create automobile contract");

    return View(model);
}


    }
}