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
        SubmitComplaint([FromForm] ComplaintDtoSubmit complaintDtoSubmit)
        {
            var email =
                HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Unauthorized();
            }
 string filePath = await SaveFile(complaintDtoSubmit.Attachment);
            var complaint =
                new Complaint {
                    UserId = user.Id,
                    Attachment = filePath,
                    Description = complaintDtoSubmit.Description,
                    ComplaintsSubject = complaintDtoSubmit.ComplaintsSubject,
                    ComplaintState = ComplaintState.Waiting,
                    ComplaintType = ComplaintType.Service,
                    User = user
                };

            _context.Complaints.Add (complaint);
            await _context.SaveChangesAsync();

            var complaintDto =
                new ComplaintDto {
                    Id = complaint.Id,
                    Attachment = complaint.Attachment,
                    Description = complaint.Description,
                    ComplaintsSubject = complaint.ComplaintsSubject,
                    ComplaintState = complaint.ComplaintState.ToString(),
                    ComplaintType = complaint.ComplaintType.ToString(),
                    UserId = complaint.UserId
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

[HttpDelete("deletecomplaint/{id}")]
[Authorize(Roles = "Member")]
public async Task<IActionResult> DeleteComplaint(long id)
{
    var email = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
        return Unauthorized("User not found.");
    }

    var complaint = await _context.Complaints.FindAsync(id);
    if (complaint == null)
    {
        return NotFound();
    }

    if (complaint.UserId != user.Id)
    {
        return Forbid("You do not have permission to delete this complaint.");
    }

    _context.Complaints.Remove(complaint);
    await _context.SaveChangesAsync();
    return NoContent();
}


[HttpPut("updatecomplaint/{id}")]
[Authorize(Roles = "Member")]
public async Task<IActionResult> UpdateComplaint(long id, [FromForm] ComplaintUpdateDto complaintUpdateDto)
{
    var email = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
        return Unauthorized("User not found.");
    }

    var complaint = await _context.Complaints.FindAsync(id);
    if (complaint == null)
    {
        return NotFound();
    }

    if (complaint.UserId != user.Id)
    {
        return Forbid("You do not have permission to update this complaint.");
    }

    // Update the complaint details
    complaint.Description = complaintUpdateDto.Description;
    complaint.ComplaintsSubject = complaintUpdateDto.ComplaintsSubject;

    // Handle file update if a new file is provided
    if (complaintUpdateDto.Attachment != null)
    {
        string filePath = await SaveFile(complaintUpdateDto.Attachment);
        complaint.Attachment = filePath; // Update the file path to the new file
    }

    await _context.SaveChangesAsync();
    return NoContent();
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

private async Task<string> SaveFile(IFormFile file)
{
    if (file == null || file.Length == 0)
    {
        return null; 
    }


    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    if (!Directory.Exists(uploadsDirectory))
    {
        Directory.CreateDirectory(uploadsDirectory);
    }


    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
    var filePath = Path.Combine(uploadsDirectory, uniqueFileName);


    using (var fileStream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(fileStream);
    }

    return uniqueFileName;
}


[HttpGet("image/{fileName}")]
public IActionResult GetImage(string fileName)
{
    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    var filePath = Path.Combine(uploadsDirectory, fileName);

    if (!System.IO.File.Exists(filePath))
    {
        return NotFound("Image not found.");
    }

    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    return File(fileStream, "image/jpeg"); 
}



    }
}
