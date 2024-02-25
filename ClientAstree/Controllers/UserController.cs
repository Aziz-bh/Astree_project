using ClientAstree.Contracts;
using ClientAstree.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientAstree.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
          private readonly IAuthenticationService _authService;

        public UserController(IUserService leaveTypeService,IAuthenticationService authService)
        {
            this._userService = leaveTypeService;
            this._authService = authService;
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
                    Role = user.Role,
                    CIN = user.CIN,
                    BirthDate = user.BirthDate,
                    Nationality = user.Nationality,
                    Gender = user.Gender,
                    Civility = user.Civility
                    // Continue mapping other fields
                };

            return View(model);
        }


        public IActionResult Login(string returnUrl = null)
        {
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
