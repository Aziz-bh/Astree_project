namespace API.DTOs
{
      public class ComplaintDtoSubmit
    {
        public IFormFile Attachment { get; set; }
        public string Description { get; set; }
        public string ComplaintsSubject { get; set; }
       
    }
}