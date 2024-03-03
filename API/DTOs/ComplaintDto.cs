namespace API.DTOs
{
public class ComplaintDto
{
    public long Id { get; set; }
    public string Attachment { get; set; }
    public string Description { get; set; }
    public string ComplaintsSubject { get; set; }
    public string ComplaintState { get; set; }
    public string ComplaintType { get; set; }
    public int UserId { get; set; }

    public string UserName { get; set; }
    public string UserEmail { get; set; }
}

}