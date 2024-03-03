using System.Security.Claims;
using API.DTOs;
using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ComplaintsController : BaseApiController
    {
        private readonly AstreeDbContext _context;

        private readonly UserManager<User> _userManager;

        public ComplaintsController(
            AstreeDbContext context,
            UserManager<User> userManager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("submit")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<ComplaintDto>>
        SubmitComplaint([FromBody] ComplaintDtoSubmit complaintDtoSubmit)
        {
            var email =
                HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Unauthorized();
            }

            var complaint =
                new Complaint {
                    UserId = user.Id,
                    Attachment = complaintDtoSubmit.Attachment,
                    Description = complaintDtoSubmit.Description,
                    ComplaintsSubject = complaintDtoSubmit.ComplaintsSubject,
                    ComplaintState = ComplaintState.Waiting,
                    ComplaintType = ComplaintType.Service,
                    User = user
                };

            _context.Complaints.Add (complaint);
            await _context.SaveChangesAsync();

            // Map the saved Complaint entity to a ComplaintDto to include the Id and other generated fields
            var complaintDto =
                new ComplaintDto {
                    Id = complaint.Id,
                    Attachment = complaint.Attachment,
                    Description = complaint.Description,
                    ComplaintsSubject = complaint.ComplaintsSubject,
                    ComplaintState = complaint.ComplaintState.ToString(),
                    ComplaintType = complaint.ComplaintType.ToString(),
                    UserId = complaint.UserId
                    // Map other properties as necessary
                };

            return CreatedAtAction(nameof(GetComplaint),
            new { id = complaint.Id },
            complaintDto);
        }

        [HttpGet("all")]
        //  [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<IEnumerable<ComplaintDto>>>
        GetAllComplaints()
        {
            var complaints =
                await _context
                    .Complaints
                    .Select(c =>
                        new ComplaintDto {
                            Id = c.Id,
                            Attachment = c.Attachment,
                            Description = c.Description,
                            ComplaintsSubject = c.ComplaintsSubject,
                            ComplaintState = c.ComplaintState.ToString(),
                            ComplaintType = c.ComplaintType.ToString(),
                            UserId = c.UserId,
                            UserName = c.User.UserName,
                            UserEmail = c.User.Email
                        })
                    .ToListAsync();

            return Ok(complaints);
        }

        [HttpPut("update/{id}")]
         [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult>
        UpdateComplaintStatus(long id, [FromBody] ComplaintState state)
        {
            var complaint = await _context.Complaints.FindAsync(id);

            if (complaint == null)
            {
                return NotFound();
            }

            complaint.ComplaintState = state;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("mycomplaints")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<IEnumerable<ComplaintDto>>>
        GetUserComplaints()
        {
            var userEmail =
                HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var complaints =
                await _context
                    .Complaints
                    .Where(c => c.UserId == user.Id)
                    .Select(c => MapToDto(c))
                    .ToListAsync();

            return Ok(complaints);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintDto>> GetComplaint(long id)
        {
            var complaint =
                await _context
                    .Complaints
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
            {
                return NotFound();
            }

            var currentUserId =
                HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


            return Ok(MapToDto(complaint));
        }


private static ComplaintDto MapToDto(Complaint complaint)
{
    return new ComplaintDto
    {
                       Id = complaint.Id,
                Attachment = complaint.Attachment,
                Description = complaint.Description,
                ComplaintsSubject = complaint.ComplaintsSubject,
                ComplaintState = complaint.ComplaintState.ToString(),
                ComplaintType = complaint.ComplaintType.ToString(),
                UserId = complaint.UserId,
                UserName = complaint.User?.UserName,
                UserEmail = complaint.User?.Email
    };
}


    }
}
