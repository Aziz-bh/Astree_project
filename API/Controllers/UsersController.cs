using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly AstreeDbContext _context;

        private readonly UserManager<User> _userManager;

        public UsersController(
            UserManager<User> userManager,
            AstreeDbContext context
        )
        {
            _context = context;
            _userManager = userManager;
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet]
public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
{
    // Fetch users excluding deleted ones
    var users = await _context.Users
        .Where(u => !(bool)u.IsDeleted)
        .ToListAsync();

    // Prepare a list to hold the UserDto objects
    var userDtos = new List<UserDto>();

    foreach (var user in users)
    {
        var roles = await _userManager.GetRolesAsync(user); 

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Picture = user.Picture,
            CIN = user.CIN,
            Gender = user.Gender.HasValue ? user.Gender.Value.ToString() : null, 
            BirthDate = user.BirthDate,
            Nationality = user.Nationality,
            Civility = user.Civility.HasValue ? user.Civility.Value.ToString() : null, 
            Roles = roles.ToList()
        };

        userDtos.Add(userDto);
    }

    return Ok(userDtos);
}


        // [Authorize(Roles = "Member, Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null || (bool) user.IsDeleted)
            {
                return NotFound();
            }


            var roles = await _userManager.GetRolesAsync(user);

            var userDto =
                new UserDto {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Picture = user.Picture,
            CIN = user.CIN,
            Gender = user.Gender.HasValue ? user.Gender.Value.ToString() : null, 
            BirthDate = user.BirthDate,
            Nationality = user.Nationality,
            Civility = user.Civility.HasValue ? user.Civility.Value.ToString() : null, 
            Roles = roles.ToList()
                };

            return Ok(userDto);
        }

        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user data");
            }

            _context.Users.Add (user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult>
        UpdateUser(int id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            if (userUpdateDTO == null)
            {
                return BadRequest("Invalid user data");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }


            if (!string.IsNullOrWhiteSpace(userUpdateDTO.FirstName))
            {
                user.FirstName = userUpdateDTO.FirstName;
            }
            if (!string.IsNullOrWhiteSpace(userUpdateDTO.LastName))
            {
                user.LastName = userUpdateDTO.LastName;
            }
            if (!string.IsNullOrWhiteSpace(userUpdateDTO.CIN))
            {
                user.CIN = userUpdateDTO.CIN;
            }
            if (!string.IsNullOrWhiteSpace(userUpdateDTO.PhoneNumber))
            {
                user.PhoneNumber = userUpdateDTO.PhoneNumber;
            }
            if (!string.IsNullOrWhiteSpace(userUpdateDTO.Picture))
            {
                user.Picture = userUpdateDTO.Picture;
            }
            if (userUpdateDTO.Gender.HasValue)
            {
                user.Gender = userUpdateDTO.Gender.Value;
            }
            if (userUpdateDTO.BirthDate.HasValue)
            {
                user.BirthDate = userUpdateDTO.BirthDate.Value;
            }
            if (!string.IsNullOrWhiteSpace(userUpdateDTO.Nationality))
            {
                user.Nationality = userUpdateDTO.Nationality;
            }
            if (userUpdateDTO.Civility.HasValue)
            {
                user.Civility = userUpdateDTO.Civility.Value;
            }

            user.UpdatedAt = DateTime.UtcNow;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // [Authorize(Roles = "Member")]
        [HttpPut("update")]
        public async Task<IActionResult>
        UpdateUser([FromBody] UserUpdateDTO userUpdateDTO)
        {

            var email =
                HttpContext
                    .User?
                    .Claims?
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                    .Value;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Invalid token information");
            }

   
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User with email {email} not found.");
            }


            if (!string.IsNullOrWhiteSpace(userUpdateDTO.FirstName))
            {
                user.FirstName = userUpdateDTO.FirstName;
            }
            if (!string.IsNullOrWhiteSpace(userUpdateDTO.LastName))
            {
                user.LastName = userUpdateDTO.LastName;
            }
            if (!string.IsNullOrWhiteSpace(userUpdateDTO.PhoneNumber))
            {
                user.PhoneNumber = userUpdateDTO.PhoneNumber;
            }
            if (!string.IsNullOrWhiteSpace(userUpdateDTO.Picture))
            {
                user.Picture = userUpdateDTO.Picture;
            }


            user.UpdatedAt = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
