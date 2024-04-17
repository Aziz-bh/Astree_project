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
            return View(new PropertyVM()); // Pass a new ViewModel to ensure the form is clean.
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyVM model)
        {
            if (ModelState.IsValid)
            {
                var createdProperty = await _propertyService.CreatePropertyAsync(model);
                if (createdProperty != null)
                {
                    return RedirectToAction("Index"); // Assuming there is an Index view to list properties
                }
                ModelState.AddModelError("", "Failed to create property contract");
            }
            return View(model);
        }
    }
}