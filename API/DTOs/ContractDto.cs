using Data.Models;

namespace API.DTOs
{
    public abstract class ContractDto
    {
        public long Id { get; set; }
        public ContractType ContractType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float Quota { get; set; }
        public int UserId { get; set; }
    }
}
