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
             private readonly IComplaintService _complaintService;
             private readonly AnalyticsService _analyticsService;

        public UserController(AnalyticsService analyticsService,IUserService leaveTypeService,IAuthenticationService authService,IAutomobileService automobileService,IPropertyService propertyService ,IComplaintService complaintService)
        {
            this._userService = leaveTypeService;
            this._authService = authService;
            this._automobileService=automobileService;
            this._propertyService=propertyService;
            this._complaintService = complaintService;
            this._analyticsService = analyticsService;
        }

        // GET: /User
public async Task<ActionResult> Index(string searchEmail = null, string searchCIN = null, string searchRole = null, int pageNumber = 1, int pageSize = 4)
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


public async Task<IActionResult> Dashboard()
{
    var automobileContracts = await _automobileService.AutomobileAllAsync();
    var propertyContracts = await _propertyService.PropertyAllAsync();
    var users = await _userService.GetUsersAsync();
    var complaints = await _complaintService.GetAllComplaintsAsync();

    var report = await _analyticsService.GetReport();

    var sessions = new List<string>();
    var totalUsers = new List<string>();
    var screenPageViews = new List<string>();
    var bounceRates = new List<string>();
    var engagementRates = new List<string>();
    var eventCounts = new List<string>();
    var conversions = new List<string>();
    var dates = new List<string>();
    var countries = new List<string>();
    var regions = new List<string>();
    var sessionSources = new List<string>();
    var sessionDefaultChannelGroupings = new List<string>();
    var pagePaths = new List<string>();
    var eventNames = new List<string>();
    var deviceCategories = new List<string>();
    var browsers = new List<string>();

    foreach (var row in report.Rows)
    {
        if (row.MetricValues.Count >= 7)
        {
            sessions.Add(row.MetricValues[0].Value);
            totalUsers.Add(row.MetricValues[1].Value);
            screenPageViews.Add(row.MetricValues[2].Value);
            bounceRates.Add(row.MetricValues[3].Value);
            engagementRates.Add(row.MetricValues[4].Value);
            eventCounts.Add(row.MetricValues[5].Value);
            conversions.Add(row.MetricValues[6].Value);
        }

        if (row.DimensionValues.Count >= 8)
        {
            dates.Add(row.DimensionValues[0].Value);
            countries.Add(row.DimensionValues[1].Value);
            regions.Add(row.DimensionValues[2].Value);
            sessionSources.Add(row.DimensionValues[3].Value);
            sessionDefaultChannelGroupings.Add(row.DimensionValues[4].Value);
            pagePaths.Add(row.DimensionValues[5].Value);
            eventNames.Add(row.DimensionValues[6].Value);
            deviceCategories.Add(row.DimensionValues[7].Value);
            browsers.Add(row.DimensionValues[8].Value);
        }
    }

    var model = new DashboardViewModel
    {
        AutomobileContractsCount = automobileContracts?.Count() ?? 0,
        PropertyContractsCount = propertyContracts?.Count() ?? 0,
        UsersCount = users?.Count() ?? 0,
        ComplaintsCount = complaints?.Count() ?? 0,
        Sessions = sessions,
        TotalUsers = totalUsers,
        ScreenPageViews = screenPageViews,
        BounceRates = bounceRates,
        EngagementRates = engagementRates,
        EventCounts = eventCounts,
        Conversions = conversions,
        Dates = dates,
        Countries = countries,
        Regions = regions,
        RegionUserCounts = regions.Select(region => regions.Count(r => r == region)).ToList(),
        SessionSources = sessionSources,
        SessionDefaultChannelGroupings = sessionDefaultChannelGroupings,
        PagePaths = pagePaths,
        EventNames = eventNames,
        DeviceCategories = deviceCategories,
        Browsers = browsers
    };

    return View(model);
}


    public IActionResult MyContract()
{
    return View();
}

public IActionResult AddContract()
{
    return View();
}


        // GET: Admin/Automobile
        public IActionResult AdminAutomobile()
        {
            return View();
        }

                // GET: Admin/PropertyManagement
        public IActionResult AdminPropertyManagement()
        {
            return View();
        }
        // POST: /User/EditRoles
[HttpPost]
public async Task<IActionResult> EditRoles(int id, string userName, string role)
{
    if (id == 0 || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(role))
    {
        TempData["ErrorMessage"] = "User ID, username, and role are required.";
        return RedirectToAction("Details", new { id = id });
    }

    var roleEditDto = new RoleEditDto
    {
        RoleNames = new List<string> { role }
    };

    try
    {
        await _userService.EditRolesAsync(userName, roleEditDto);
        TempData["SuccessMessage"] = "User role updated successfully!";
    }
    catch (Exception ex)
    {
        TempData["ErrorMessage"] = $"Error updating user role: {ex.Message}";
    }

    return RedirectToAction("Details", new { id = id });
    
}


        public IActionResult Mydues()
        {
            return View();
        }

         public IActionResult ViewContracts()
        {
            return View();
        }


        public async Task<IActionResult> Edit(int id)
{
    var user = await _userService.GetUserAsync(id); // Implement GetUserById in your service

    if (user == null)
    {
        return NotFound();
    }

    var model = new UserUpdateDTO
    {
        FirstName = user.FirstName,
        LastName = user.LastName,
        Cin = user.CIN,
        PhoneNumber = user.PhoneNumber,
        Gender = Enum.Parse<UserGender>(user.Gender),
        BirthDate = user.BirthDate,
        Nationality = user.Nationality,
        Civility = Enum.Parse<CivilStatus>(user.Civility)
    };

    return View(model);
}

[HttpPost]
public async Task<IActionResult> Edit(UserUpdateDTO model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    try
    {
        await _userService.UpdateAsync(model);
        TempData["SuccessMessage"] = "User updated successfully!";
    }
    catch (Exception ex)
    {
        TempData["ErrorMessage"] = "Error updating user: " + ex.Message;
        return View(model);
    }

    return RedirectToAction("Index");
}

        public IActionResult Agencies()
        {
            // Sample data for demonstration purposes
            var agencies = new List<AgencyVM>
            {
                new AgencyVM { Id = 1, Name = "Astree Assurance Tunis", Address = "Ave Habib Bourguiba, Tunis", Latitude = 36.8065, Longitude = 10.1815 },
                new AgencyVM { Id = 2, Name = "Astree Assurance Sfax", Address = "Ave Hedi Chaker, Sfax", Latitude = 34.7398, Longitude = 10.7603 },
                new AgencyVM { Id = 3, Name = "Astree Assurance Sousse", Address = "Ave 14 Janvier, Sousse", Latitude = 35.8256, Longitude = 10.636 },
                new AgencyVM { Id = 4, Name = "Astree Assurance Nabeul", Address = "Ave Habib Thameur, Nabeul", Latitude = 36.4561, Longitude = 10.7376 }
            };

            return View(agencies);
        }

    }
}
