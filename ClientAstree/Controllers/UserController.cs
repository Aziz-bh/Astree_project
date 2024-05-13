using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services;
using ClientAstree.Services.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
public async Task<ActionResult> Index(string searchEmail = null, string searchCIN = null, string searchRole = null, int pageNumber = 1, int pageSize = 6)
{
    var users = await _userService.GetUsersAsync();
    
    // Apply search and filter
    if (!string.IsNullOrEmpty(searchEmail))
    {
        users = users.Where(u => u.Email.Contains(searchEmail, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    if (!string.IsNullOrEmpty(searchCIN))
    {
        users = users.Where(u => u.CIN.Contains(searchCIN, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    if (!string.IsNullOrEmpty(searchRole))
    {
        users = users.Where(u => u.Roles.Any(r => r.Contains(searchRole, StringComparison.OrdinalIgnoreCase))).ToList();
    }

    // Apply pagination
    var pagedUsers = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

    ViewBag.PageNumber = pageNumber;
    ViewBag.PageSize = pageSize;
    ViewBag.TotalPages = (int)Math.Ceiling(users.Count / (double)pageSize);

    return View(pagedUsers);
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
    


    // GET: /User/Profile
    public async Task<IActionResult> Profile()
    {
                ViewBag.CivilityOptions = Enum.GetValues(typeof(CivilStatus))
            .Cast<CivilStatus>()
            .Select(c => new SelectListItem { Text = c.ToString(), Value = c.ToString() })
            .ToList();

        ViewBag.GenderOptions = Enum.GetValues(typeof(UserGender))
            .Cast<UserGender>()
            .Select(g => new SelectListItem { Text = g.ToString(), Value = g.ToString() })
            .ToList();

        var userProfile = await _userService.ProfileAsync();
        if (userProfile == null)
        {
            return NotFound("User profile is not available.");
        }

        int userId = userProfile.Id ?? 0; // Convert nullable integer to non-nullable integer

        var automobileContracts = await _automobileService.GetUserAutomobiles(userId);
           var propertyContracts=await _propertyService.GetUserPropertys(userId);

               ViewBag.AutomobileContractCount = automobileContracts?.Count ?? 0;
    ViewBag.PropertyContractCount = propertyContracts?.Count ?? 0;

           
        return View(userProfile);
    }


// POST: /User/UpdateProfile
[HttpPost]
public async Task<IActionResult> UpdateProfile(UserUpdateDTO model)
{
 await _userService.UpdateAsync(model);
    if (!ModelState.IsValid)
    {
          Console.WriteLine("ModelState");
        return View(model); // or return to an error view or reload the profile page with error messages
    }
Console.WriteLine("try");
    try
    {
        Console.WriteLine("await");
        await _userService.UpdateAsync(model);
        TempData["SuccessMessage"] = "Profile updated successfully!";
         Console.WriteLine("Profile");
    }
    catch (Exception ex)
    {
         Console.WriteLine("catch");
        // Log the exception details
        TempData["ErrorMessage"] = "Error updating profile: " + ex.Message;
        return View("Profile", model); // Return to the profile page with the filled model and error message
    }

    return RedirectToAction("Profile"); // Redirect back to the profile page or another relevant page
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
