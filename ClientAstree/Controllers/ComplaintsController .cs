using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace ClientAstree.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly IComplaintService _complaintService;

        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchSubject = null, string searchStatus = null, string searchUserEmail = null, int pageNumber = 1, int pageSize = 10)
        {
            var complaintDtos = await _complaintService.GetAllComplaintsAsync();
            var complaintVms = complaintDtos.Select(dto => new ComplaintVM
            {
                Id = dto.Id,
                Attachment = dto.Attachment,
                Description = dto.Description,
                ComplaintsSubject = dto.ComplaintsSubject,
                ComplaintState = dto.ComplaintState,
                ComplaintType = dto.ComplaintType,
                UserId = dto.UserId,
                UserName = dto.UserName,
                UserEmail = dto.UserEmail
            }).ToList() ?? new List<ComplaintVM>();

            // Apply search and filter
            if (!string.IsNullOrEmpty(searchSubject))
            {
                complaintVms = complaintVms.Where(c => c.ComplaintsSubject?.Contains(searchSubject, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchStatus))
            {
                complaintVms = complaintVms.Where(c => c.ComplaintState?.Contains(searchStatus, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchUserEmail))
            {
                complaintVms = complaintVms.Where(c => c.UserEmail?.Contains(searchUserEmail, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            // Apply pagination
            var pagedComplaints = complaintVms.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(complaintVms.Count / (double)pageSize);

            return View(pagedComplaints);
        }

        [HttpGet]
        public IActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ComplaintDtoSubmit model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var attachment = new FileParameter(model.Attachment.OpenReadStream(), model.Attachment.FileName, model.Attachment.ContentType);
            await _complaintService.SubmitComplaintAsync(attachment, model.Description, model.ComplaintsSubject);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> MyComplaints(string searchSubject = null, string searchStatus = null, string searchUserEmail = null, int pageNumber = 1, int pageSize = 10)
        {
            var complaintDtos = await _complaintService.GetUserComplaintsAsync();
            var complaintVms = complaintDtos.Select(dto => new ComplaintVM
            {
                Id = dto.Id,
                Attachment = dto.Attachment,
                Description = dto.Description,
                ComplaintsSubject = dto.ComplaintsSubject,
                ComplaintState = dto.ComplaintState,
                ComplaintType = dto.ComplaintType,
                UserId = dto.UserId,
                UserName = dto.UserName,
                UserEmail = dto.UserEmail
            }).ToList() ?? new List<ComplaintVM>();

            // Apply search and filter
            if (!string.IsNullOrEmpty(searchSubject))
            {
                complaintVms = complaintVms.Where(c => c.ComplaintsSubject?.Contains(searchSubject, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchStatus))
            {
                complaintVms = complaintVms.Where(c => c.ComplaintState?.Contains(searchStatus, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchUserEmail))
            {
                complaintVms = complaintVms.Where(c => c.UserEmail?.Contains(searchUserEmail, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            // Apply pagination
            var pagedComplaints = complaintVms.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(complaintVms.Count / (double)pageSize);

            return View("MyComplaints", pagedComplaints);
        }

[HttpGet]
public async Task<IActionResult> EditComplaint(long id)
{
    var complaint = await _complaintService.GetComplaintAsync(id);
    if (complaint == null)
    {
        return NotFound();
    }
    return View(complaint);
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(long id, ComplaintDtoSubmit model)
{
    if (!ModelState.IsValid)
    {
        // Fetch the existing complaint to display in the view if model state is invalid
        var existingComplaint = await _complaintService.GetComplaintAsync(id);
        if (existingComplaint == null)
        {
            return NotFound();
        }
        return View(new ComplaintDto
        {
            Id = existingComplaint.Id,
            Attachment = existingComplaint.Attachment,
            Description = model.Description ?? existingComplaint.Description,
            ComplaintsSubject = model.ComplaintsSubject ?? existingComplaint.ComplaintsSubject,
            ComplaintState = existingComplaint.ComplaintState,
            ComplaintType = existingComplaint.ComplaintType,
            UserId = existingComplaint.UserId,
            UserName = existingComplaint.UserName,
            UserEmail = existingComplaint.UserEmail
        });
    }

    var attachment = model.Attachment != null ? new FileParameter(model.Attachment.OpenReadStream(), model.Attachment.FileName, model.Attachment.ContentType) : null;
    await _complaintService.UpdateComplaintAsync(id, attachment, model.Description, model.ComplaintsSubject);
    return RedirectToAction(nameof(MyComplaints));
}



[HttpGet]
public async Task<IActionResult> Delete(long id)
{
    await _complaintService.DeleteComplaintAsync(id);
    return RedirectToAction(nameof(MyComplaints));
}


[HttpGet]
public async Task<IActionResult> Admin(string searchSubject = null, string searchStatus = null, string searchUserEmail = null, int pageNumber = 1, int pageSize = 10)
        {
            var complaintDtos = await _complaintService.GetAllComplaintsAsync();
            var complaintVms = complaintDtos.Select(dto => new ComplaintVM
            {
                Id = dto.Id,
                Attachment = dto.Attachment,
                Description = dto.Description,
                ComplaintsSubject = dto.ComplaintsSubject,
                ComplaintState = dto.ComplaintState,
                ComplaintType = dto.ComplaintType,
                UserId = dto.UserId,
                UserName = dto.UserName,
                UserEmail = dto.UserEmail
            }).ToList() ?? new List<ComplaintVM>();

            // Apply search and filter
            if (!string.IsNullOrEmpty(searchSubject))
            {
                complaintVms = complaintVms.Where(c => c.ComplaintsSubject?.Contains(searchSubject, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchStatus))
            {
                complaintVms = complaintVms.Where(c => c.ComplaintState?.Contains(searchStatus, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchUserEmail))
            {
                complaintVms = complaintVms.Where(c => c.UserEmail?.Contains(searchUserEmail, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            // Apply pagination
            var pagedComplaints = complaintVms.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(complaintVms.Count / (double)pageSize);

            return View(pagedComplaints);
        }

[HttpPost]
public async Task<IActionResult> UpdateComplaintStatus(long id, ComplaintState state)
{
    await _complaintService.UpdateComplaintStatusAsync(id, state);
    return RedirectToAction(nameof(Admin));
}


    }
    
}