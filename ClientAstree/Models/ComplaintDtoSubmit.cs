namespace ClientAstree.Models
{
    public class ComplaintDtoSubmit
    {
        public long? Id { get; set; }  
        public IFormFile Attachment { get; set; }
        public string Description { get; set; }
        public string ComplaintsSubject { get; set; }
    }
}