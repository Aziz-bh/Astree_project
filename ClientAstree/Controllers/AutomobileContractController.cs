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
            return View(new AutomobileVM()); // Pass a new ViewModel to ensure the form is clean.
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AutomobileVM model)
        {
            if (ModelState.IsValid)
            {
                var createdAutomobile = await _automobileService.CreateAutomobileAsync(model);
                if (createdAutomobile != null)
                {
                    return RedirectToAction("Index"); // Assuming there is an Index view to list automobiles
                }
                ModelState.AddModelError("", "Failed to create automobile contract");
            }
            return View(model);
        }
    }
}