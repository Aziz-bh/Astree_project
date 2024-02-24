using ClientAstree.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ClientAstree.Controllers
{
    public class UserController : Controller
    {


        private readonly IUserService _userService;

        public UserController(IUserService leaveTypeService)
        {
            this._userService = leaveTypeService;
        }

        // GET: /User
        public async Task<ActionResult> Index()
        {
              var model = await _userService.GetUsersAsync();
            return View(model);
        }

        // GET: /User/Details/{id}
        public IActionResult Details(int id)
        {
            // TODO: Implement logic to retrieve and display user details based on the provided id
            return View();
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
