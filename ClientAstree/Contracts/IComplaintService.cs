using ClientAstree.Services.Base;

namespace ClientAstree.Contracts
{
    public interface IComplaintService
    {
        Task<ComplaintDto> SubmitComplaintAsync(FileParameter attachment, string description, string complaintsSubject);
        Task<IEnumerable<ComplaintDto>> GetAllComplaintsAsync();
        Task UpdateComplaintStatusAsync(long id, ComplaintState state);
        Task<IEnumerable<ComplaintDto>> GetUserComplaintsAsync();
        Task<ComplaintDto> GetComplaintAsync(long id);
        Task DeleteComplaintAsync(long id);
        Task UpdateComplaintAsync(long id, FileParameter attachment, string description, string complaintsSubject);
        Task<byte[]> GetComplaintAttachmentAsync(string fileName);
    }
}