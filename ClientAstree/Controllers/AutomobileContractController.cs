using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services.Base;
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
        public async Task<IActionResult> Index(string searchMake = null, string searchModel = null, string searchVehicleType = null, int pageNumber = 1, int pageSize = 10)
        {
            var contracts = await _automobileService.GetMyAutomobileContractsAsync() ?? new List<AutomobileVM>();

            // Apply search and filter
            if (!string.IsNullOrEmpty(searchMake))
            {
                contracts = contracts.Where(c => c.VehicleMake?.Contains(searchMake, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchModel))
            {
                contracts = contracts.Where(c => c.Model?.Contains(searchModel, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchVehicleType))
            {
                contracts = contracts.Where(c => c.VehicleType?.Contains(searchVehicleType, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            // Apply pagination
            var pagedContracts = contracts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(contracts.Count / (double)pageSize);

            return View(pagedContracts);
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
    try
    {
        // Manual extraction as a fallback or to ensure correct binding
        model.VehicleMake = model.VehicleMake ?? form["VehicleMake"];
        model.Model = model.Model ?? form["Model"];
        model.VehicleType = form["VehicleType"];
        model.RegistrationNumber = model.RegistrationNumber ?? form["RegistrationNumber"];
        model.ContractType = model.ContractType ?? form["ContractType"];

        if (DateTimeOffset.TryParse(form["RegistrationDate"], out var registrationDate)) {
            model.RegistrationDate = registrationDate;
        }
        if (DateTimeOffset.TryParse(form["StartDate"], out var startDate)) {
            model.StartDate = startDate;
        }
        if (DateTimeOffset.TryParse(form["EndDate"], out var endDate)) {
            model.EndDate = endDate;
        }

        model.EnginePower = int.TryParse(form["EnginePower"], out var enginePower) ? enginePower : 0;
        model.SeatsNumber = int.TryParse(form["SeatsNumber"], out var seatsNumber) ? seatsNumber : 0;
        model.VehicleValue = float.TryParse(form["VehicleValue"], out var vehicleValue) ? vehicleValue : 0;
        model.TrueVehicleValue = float.TryParse(form["TrueVehicleValue"], out var trueVehicleValue) ? trueVehicleValue : 0;

        var guarantees = form["Guarantees"].Select(int.Parse).ToArray();
        model.Guarantees = guarantees.Sum().ToString();

        // Additional server-side validation
        if (model.EndDate <= model.StartDate)
        {
            ModelState.AddModelError("", "End date must be later than start date.");
        }
        if (model.VehicleValue <= 0)
        {
            ModelState.AddModelError("", "Vehicle value must be greater than zero.");
        }

        if (ModelState.IsValid)
        {
            var createdAutomobile = await _automobileService.CreateAutomobileAsync(model);
            if (createdAutomobile != null)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Failed to create automobile contract");
        }
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", $"An error occurred while creating the automobile contract: {ex.Message}");
    }

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
     Console.WriteLine("ModelState is invalid  garanteees: " +automobile.Guarantees);
     foreach(var auto in automobile.GuaranteesList){
         Console.WriteLine("ModelState is invalid  garanteees: " +automobile.Guarantees);
     }
    return View(automobile);
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Update(AutomobileVM model, IFormCollection form)
{
    try
    {
        model.Id = long.Parse(form["Id"]);
        model.VehicleMake = form["VehicleMake"];
        model.Model = form["Model"];
        model.VehicleType = form["VehicleType"];
        if(model.VehicleType=="0"){
            //  model.VehicleType= VehicleType.Personal;
              model.VehicleType= "Personal";
        }else{
            // model.VehicleType= VehicleType.Business;
            model.VehicleType= "Business";
        }
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

        // Additional server-side validation
        if (model.EndDate <= model.StartDate)
        {
            ModelState.AddModelError("", "End date must be later than start date.");
        }
        if (model.VehicleValue <= 0)
        {
            ModelState.AddModelError("", "Vehicle value must be greater than zero.");
        }

      var c =  await _automobileService.GetAutomobileByIdAsync(model.Id);

if(model.Guarantees == "0"){
    model.Guarantees = c.Guarantees;
}
await _automobileService.UpdateAutomobileAsync(model);
            return RedirectToAction(nameof(Index));
        
    }
    catch (Exception ex)
    {
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