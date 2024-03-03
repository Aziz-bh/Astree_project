namespace API.Models
{
 public enum ComplaintState
{
    Approved,
    Rejected,
    Waiting
}

public enum ComplaintType
{
    Service,
    Technical
}

public class Complaint
{
    public long Id { get; set; }
    public string Attachment { get; set; }
    public string Description { get; set; }
    public string ComplaintsSubject { get; set; }
    public ComplaintState ComplaintState { get; set; }
    public ComplaintType ComplaintType { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}

}