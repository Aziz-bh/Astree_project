using API.DTOs;
using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
          private readonly AstreeDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(
            AstreeDbContext context, 
            UserManager<User> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await (
                from user in _context.Users orderby user.UserName
                select new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = (from userRole in user.UserRoles
                                join role in _context.Roles 
                                    on userRole.RoleId equals role.Id
                            select role.Name).ToList()
                }).ToListAsync();
            return Ok(userList);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto) 
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null) 
            {
                return BadRequest($"{userName} not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleEditDto.RoleNames;

            selectedRoles = selectedRoles ?? new string[] {};

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) 
            {
                return BadRequest("Failed to add to roles");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) 
            {
                return BadRequest("Failed to remove the roles");
            }   

            return Ok(await _userManager.GetRolesAsync(user));         
        }
    }
}