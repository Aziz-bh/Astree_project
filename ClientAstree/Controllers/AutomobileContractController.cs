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

        var createdAutomobile = await _automobileService.CreateAutomobileAsync(model);
        if (createdAutomobile != null)
        {
            return RedirectToAction(nameof(Index));
        }
        ModelState.AddModelError("", "Failed to create automobile contract");

    return View(model);
}


[HttpGet]
public async Task<IActionResult> Update(long id)
{
    var automobile = await _automobileService.GetAutomobileByIdAsync(id);
    if (automobile == null)
    {
        return NotFound("Automobile not found.");
    }
    return View(automobile);
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Update(AutomobileVM model, IFormCollection form)
{
    // Log received data for debugging

    
    // Manual extraction to ensure correct binding especially if model binding fails
    model.Id = long.Parse(form["Id"]);
    model.VehicleMake = form["VehicleMake"];
    model.Model = form["Model"];
    model.VehicleType = form["VehicleType"];
    model.RegistrationNumber = form["RegistrationNumber"];
    model.RegistrationDate = DateTimeOffset.TryParse(form["RegistrationDate"], out var regDate) ? regDate : model.RegistrationDate;
    model.StartDate = DateTimeOffset.TryParse(form["StartDate"], out var startDate) ? startDate : model.StartDate;
    model.EndDate = DateTimeOffset.TryParse(form["EndDate"], out var endDate) ? endDate : model.EndDate;
    model.EnginePower = int.TryParse(form["EnginePower"], out var enginePower) ? enginePower : model.EnginePower;
    model.SeatsNumber = int.TryParse(form["SeatsNumber"], out var seatsNumber) ? seatsNumber : model.SeatsNumber;
    model.VehicleValue = float.TryParse(form["VehicleValue"], out var vehicleValue) ? vehicleValue : model.VehicleValue;
    model.TrueVehicleValue = float.TryParse(form["TrueVehicleValue"], out var trueVehicleValue) ? trueVehicleValue : model.TrueVehicleValue;
          var guarantees = form["Guarantees"].Select(x => int.Parse(x)).Sum();
    model.Guarantees = guarantees.ToString();

    Console.WriteLine($"Updating Automobile ID: {model.Id} with Guarantees: {model.Guarantees}");

      Console.WriteLine($"Attempting to update Automobile ID: {model.Id}");
    Console.WriteLine($"Model Data: VehicleMake={model.VehicleMake}, Model={model.Model}, SeatsNumber={model.SeatsNumber}, EnginePower={model.EnginePower}");
  //   Console.WriteLine($"Attempting to update Automobile ID: {model.GuaranteesList}");
    //   Console.WriteLine($"Attempting to update Automobile ID: {model.GuaranteesList}");
      //   Console.WriteLine($"Attempting to update Automobile ID: {model.GuaranteesList}");
   foreach (var item in model.GuaranteesList)
    {
        Console.WriteLine($"Guarantees: {item}");
    }

   
   
   
    // Clear and revalidate model state after manual assignment


    try
    {
              Console.WriteLine($"Attempting to update Automobile ID: {model.Id}");
    Console.WriteLine($"Model Data: VehicleMake={model.VehicleMake}, Model={model.Model}, SeatsNumber={model.SeatsNumber}, EnginePower={model.EnginePower}");
        // Attempt to update the automobile
        Console.WriteLine($"Updating Automobile: {model.VehicleMake}, {model.Model}, ID: {model.Id}");
        await _automobileService.UpdateAutomobileAsync(model);

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating automobile: {ex.Message}");
        ModelState.AddModelError("", $"An error occurred while updating the automobile: {ex.Message}");
    }

    return View(model);
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(long id)
{
    await _automobileService.DeleteAutomobileAsync(id);
    return RedirectToAction("Index"); // Redirect to the listing page after deletion
}



    }
}