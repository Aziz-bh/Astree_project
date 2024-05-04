using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClientAstree.Models;
using ClientAstree.Services.Base;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClientAstree.Contracts;

namespace ClientAstree.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAuthenticationService _authService;

    public HomeController(ILogger<HomeController> logger, IAuthenticationService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.CivilityOptions = Enum.GetValues(typeof(CivilStatus))
            .Cast<CivilStatus>()
            .Select(c => new SelectListItem { Text = c.ToString(), Value = c.ToString() })
            .ToList();

        ViewBag.GenderOptions = Enum.GetValues(typeof(UserGender))
            .Cast<UserGender>()
            .Select(g => new SelectListItem { Text = g.ToString(), Value = g.ToString() })
            .ToList();

      return View(new RegisterVM());
    }

[HttpPost]
public async Task<IActionResult> Register(RegisterVM model)
{
    // Prepare dropdown data for Civility and Gender options regardless of the post outcome
    ViewBag.CivilityOptions = Enum.GetValues(typeof(CivilStatus))
        .Cast<CivilStatus>()
        .Select(c => new SelectListItem { Text = c.ToString(), Value = c.ToString() })
        .ToList();

    ViewBag.GenderOptions = Enum.GetValues(typeof(UserGender))
        .Cast<UserGender>()
        .Select(g => new SelectListItem { Text = g.ToString(), Value = g.ToString() })
        .ToList();

    if (!ModelState.IsValid)
    {
        return View(model); // Return the view with current model to show validation errors
    }
    var cont=model.Nationality+","+model.ConcatenatedNationality;
    RegisterDto registrationRequest = new RegisterDto
    {
        Email = model.Email,
        Password = model.Password,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Cin = model.Cin,
        PhoneNumber = model.PhoneNumber,
        BirthDate = model.BirthDate,
        Nationality = cont,
        Civility = model.Civility,
        Gender = model.Gender
    };
    try
    {
        bool isSuccess = await _authService.Register(registrationRequest);
        if (isSuccess)
        {
            ViewBag.RegistrationSuccess = "Please verify your email to log in.";
            return RedirectToAction("Login", "User", new { message = "Please verify your email to log in." });

        }
                else
        {                ModelState.AddModelError("Email", "The email address is already in use.");

          
        }
    }
    catch (ApiException ex)
    {
        if (ex.StatusCode == 400 && ex.Message.Contains("Email is taken"))
        {
            ModelState.AddModelError("Email", "The email address is already taken.");
        }
        else
        {
            ModelState.AddModelError("", "Registration failed due to an unexpected error.");
        }
    }

    // If we reach here, something failed, redisplay form
    return View(model);
}
        
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
