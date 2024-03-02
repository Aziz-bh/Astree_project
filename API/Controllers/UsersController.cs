using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

  [Authorize]
    public class UsersController:BaseApiController
    {
        private readonly AstreeDbContext _context;

        public UsersController(AstreeDbContext context)
        {
            _context = context;
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users
    .Where(u => (bool)!u.IsDeleted)
    .ToListAsync();
            return users;
        }


        
        [Authorize(Roles = "Member")]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

    }
}