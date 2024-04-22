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
        public async Task<IActionResult> Index()
        {
            var complaintDtos  = await _complaintService.GetAllComplaintsAsync();
              var complaintVms = complaintDtos.Select(dto => new ComplaintVM
    {
        // Map properties from dto to vm
        Id = dto.Id,
        Attachment = dto.Attachment,
        Description = dto.Description,
        ComplaintsSubject = dto.ComplaintsSubject,
        ComplaintState = dto.ComplaintState,
        ComplaintType = dto.ComplaintType,
        UserId = dto.UserId,
        UserName = dto.UserName,
        UserEmail = dto.UserEmail
    }).ToList();
            return View(complaintVms);
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
        public async Task<IActionResult> MyComplaints()
        {
            var complaints = await _complaintService.GetUserComplaintsAsync();
            return View(complaints);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
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
                return View(model);
            }

            var attachment = model.Attachment != null ? new FileParameter(model.Attachment.OpenReadStream(), model.Attachment.FileName, model.Attachment.ContentType) : null;
            await _complaintService.UpdateComplaintAsync(id, attachment, model.Description, model.ComplaintsSubject);
            return RedirectToAction(nameof(MyComplaints));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            await _complaintService.DeleteComplaintAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}