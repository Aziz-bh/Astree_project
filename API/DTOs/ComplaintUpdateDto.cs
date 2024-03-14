namespace API.DTOs
{
using Microsoft.AspNetCore.Http;

public class ComplaintUpdateDto
{
    public IFormFile Attachment { get; set; }
    public string Description { get; set; }
    public string ComplaintsSubject { get; set; }
    // Add other updatable fields as necessary
}

}