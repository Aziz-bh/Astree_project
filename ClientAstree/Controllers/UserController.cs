using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientAstree.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
          private readonly IAuthenticationService _authService;
            private readonly IAutomobileService _automobileService;
            private readonly IPropertyService _propertyService;

        public UserController(IUserService leaveTypeService,IAuthenticationService authService,IAutomobileService automobileService,IPropertyService propertyService)
        {
            this._userService = leaveTypeService;
            this._authService = authService;
            this._automobileService=automobileService;
            this._propertyService=propertyService;
        }

        // GET: /User
        public async Task<ActionResult> Index()
        {
            var model = await _userService.GetUsersAsync();
            return View(model);
        }

        // GET: /User/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserAsync(id); // Implement GetUserById in your service
           var automobileContracts=await _automobileService.GetUserAutomobiles(id);
           var propertyContracts=await _propertyService.GetUserPropertys(id);

           
           
            if (user == null)
            {
                return NotFound();
            }

            var model =
                new UserVM {
                    // Map your user data to the UserVM model
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = user.Roles,
                    CIN = user.CIN,
                    BirthDate = user.BirthDate,
                    Nationality = user.Nationality,
                    Gender = user.Gender,
                    Civility = user.Civility
                    // Continue mapping other fields
                };
                    var viewModel = new UserDetailsViewModel
    {
        User = model,
        AutomobileContracts = automobileContracts,
        PropertyContracts = propertyContracts
    };

            return View(viewModel);
        }
              //  GET: /User/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Implement logic to retrieve and display the user delete confirmation page based on the provided id
           
          await  _userService.UsersDELETEAsync(id);
            return RedirectToAction("Index");
        }


        //      // POST: /User/Delete/{id}
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            // TODO: Implement logic to delete the user based on the provided id
            return RedirectToAction("Index");
        }


public IActionResult Login(string message = "")
{
    ViewBag.Message = message;
    return View();
}

         [HttpPost]
        public async Task<IActionResult> Login(LoginVM login, string returnUrl)
        {
            
   
                returnUrl ??= Url.Content("~/");
                var isLoggedIn = await _authService.Authenticate(login.Email, login.Password);
                if (isLoggedIn)
                    return LocalRedirect(returnUrl);
         
            ModelState.AddModelError("", "Log In Attempt Failed. Please try again.");
            return View(login);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            await _authService.Logout();
            return LocalRedirect(returnUrl);
        }
    

        // // GET: /User/Create
        // public IActionResult Create()
        // {
        //     // TODO: Implement logic to display the user creation form
        //     return View();
        // }

        // // POST: /User/Create
        // [HttpPost]
        // public IActionResult Create(UserModel user)
        // {
        //     // TODO: Implement logic to create a new user based on the provided UserModel
        //     return RedirectToAction("Index");
        // }

        // GET: /User/Edit/{id}
        // public IActionResult Edit(int id)
        // {
        //     // TODO: Implement logic to retrieve and display the user edit form based on the provided id
        //     return View();
        // }

        // POST: /User/Edit/{id}
        // [HttpPost]
        // public IActionResult Edit(int id, UserModel user)
        // {
        //     // TODO: Implement logic to update the user based on the provided id and UserModel
        //     return RedirectToAction("Index");
        // }

        // GET: /User/Delete/{id}
        // public IActionResult Delete(int id)
        // {
        //     // TODO: Implement logic to retrieve and display the user delete confirmation page based on the provided id
        //     return View();
        // }

        // POST: /User/Delete/{id}
        // [HttpPost]
        // public IActionResult DeleteConfirmed(int id)
        // {
        //     // TODO: Implement logic to delete the user based on the provided id
        //     return RedirectToAction("Index");
        // }
    }
}
