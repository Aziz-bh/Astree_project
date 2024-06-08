using AutoMapper;
using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services.Base;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientAstree.Services
{
    public class ComplaintService : BaseHttpService, IComplaintService
    {
        private readonly IMapper _mapper;
        private readonly IClient _httpClient;
        private readonly BadWordFilterService _badWordFilterService;

        public ComplaintService(IMapper mapper, IClient httpclient, BadWordFilterService badWordFilterService, IHttpContextAccessor httpContextAccessor)
            : base(httpclient, httpContextAccessor)
        {
            _mapper = mapper;
            _httpClient = httpclient;
            _badWordFilterService = badWordFilterService;
        }

        public async Task<ComplaintDto> SubmitComplaintAsync(FileParameter attachment, string description, string complaintsSubject)
        {
            AddBearerToken();
            var filteredDescription = _badWordFilterService.FilterBadWords(description);
            return await _httpClient.SubmitAsync(attachment, filteredDescription, complaintsSubject);
        }

        public async Task<IEnumerable<ComplaintDto>> GetAllComplaintsAsync()
        {
            AddBearerToken();
            return await _httpClient.AllAsync();
        }

        public async Task UpdateComplaintStatusAsync(long id, ComplaintState state)
        {
            AddBearerToken();
            await _httpClient.UpdateStateAsync(id, state);
        }

        public async Task<IEnumerable<ComplaintDto>> GetUserComplaintsAsync()
        {
            AddBearerToken();
            return await _httpClient.MycomplaintsAsync();
        }

        public async Task<ComplaintDto> GetComplaintAsync(long id)
        {
            AddBearerToken();
            return await _httpClient.ComplaintsAsync(id);
        }

        public async Task DeleteComplaintAsync(long id)
        {
            AddBearerToken();
            await _httpClient.DeletecomplaintAsync(id);
        }

        public async Task UpdateComplaintAsync(long id, FileParameter attachment, string description, string complaintsSubject)
        {
            AddBearerToken();
            var filteredDescription = _badWordFilterService.FilterBadWords(description);
            await _httpClient.UpdatecomplaintAsync(id, attachment, filteredDescription, complaintsSubject);
        }

        public async Task<byte[]> GetComplaintAttachmentAsync(string fileName)
        {
            AddBearerToken();
            return null;
        }
    }
}
