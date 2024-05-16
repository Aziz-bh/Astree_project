using AutoMapper;
using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services.Base;

namespace ClientAstree.Services
{
    public class ComplaintService : BaseHttpService, IComplaintService
    {
        private readonly IMapper _mapper;
        private readonly IClient _httpClient;
         private readonly ILocalStorageService _localStorageService;

           public ComplaintService(IMapper mapper, IClient httpclient, ILocalStorageService localStorageService) : base(httpclient, localStorageService)
        {
            this._localStorageService = localStorageService;
            this._mapper = mapper;
            this._httpClient = httpclient;
        }

        public async Task<ComplaintDto> SubmitComplaintAsync(FileParameter attachment, string description, string complaintsSubject)
        {AddBearerToken();
            return await _httpClient.SubmitAsync(attachment, description, complaintsSubject);
        }

        public async Task<IEnumerable<ComplaintDto>> GetAllComplaintsAsync()
        {AddBearerToken();
            return await _httpClient.AllAsync();
        }

        public async Task UpdateComplaintStatusAsync(long id, ComplaintState state)
        {AddBearerToken();
            await _httpClient.UpdateStateAsync(id, state);
        }

        public async Task<IEnumerable<ComplaintDto>> GetUserComplaintsAsync()
        {AddBearerToken();
            return await _httpClient.MycomplaintsAsync();
        }

        public async Task<ComplaintDto> GetComplaintAsync(long id)
        {AddBearerToken();
            return await _httpClient.ComplaintsAsync(id);
        }

        public async Task DeleteComplaintAsync(long id)
        {AddBearerToken();
            await _httpClient.DeletecomplaintAsync(id);
        }

        public async Task UpdateComplaintAsync(long id, FileParameter attachment, string description, string complaintsSubject)
        {AddBearerToken();
            Console.WriteLine("Service : "+ id);
            
            Console.WriteLine("Service : "+ id);
            await _httpClient.UpdatecomplaintAsync(id, attachment, description, complaintsSubject);
        }

        public async Task<byte[]> GetComplaintAttachmentAsync(string fileName)
        {AddBearerToken();
            return null;
        }
    }
}