using ClientAstree.Contracts;
using ClientAstree.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientAstree.Controllers
{
    public class PropertyContractController: Controller
    {
                private readonly IUserService _userService;
          private readonly IAuthenticationService _authService;
            private readonly IAutomobileService _automobileService;
            private readonly IPropertyService _propertyService;

        public PropertyContractController(IUserService leaveTypeService,IAuthenticationService authService,IAutomobileService automobileService,IPropertyService propertyService)
        {
            this._userService = leaveTypeService;
            this._authService = authService;
            this._automobileService=automobileService;
            this._propertyService=propertyService;
        }

        [HttpGet]
public async Task<IActionResult> Index()
{
    var contracts = await _propertyService.GetMyPropertyContractsAsync();
    return View(contracts);
}


        [HttpGet]
        public IActionResult Create()
        {
            return View(new PropertyVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyVM model)
        {
            Console.WriteLine($"Creating Property: Location={model.Location}, Value={model.PropertyValue}, Start={model.StartDate}, End={model.EndDate}, Type={model.Type}, Coverage={model.Coverage}");

            if (ModelState.IsValid)
            {
                var createdProperty = await _propertyService.CreatePropertyAsync(model);
                if (createdProperty != null)
                {
                    return RedirectToAction(nameof(Index)); // Navigate to the listing page upon successful creation
                }
                ModelState.AddModelError("", "Failed to create property contract");
            }
            else
            {
                Console.WriteLine("ModelState is invalid");
                foreach (var error in ModelState.Values)
                {
                    foreach (var subError in error.Errors)
                    {
                        Console.WriteLine(subError.ErrorMessage);
                    }
                }
            }
            return View(model);
        }

                [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PropertyVM model)
        {
            if (ModelState.IsValid)
            {
                await _propertyService.UpdatePropertyAsync(model);

            }
            return View(model);
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(long id)
{
    await _propertyService.DeletePropertyAsync(id);
    return RedirectToAction("Index"); // Redirect to the listing page after deletion
}

    }
}