using AutoMapper;
using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services.Base;

namespace ClientAstree.Services
{
    public class AutomobileService: BaseHttpService,IAutomobileService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IMapper _mapper;
        private readonly IClient _httpclient;
                 public AutomobileService(IMapper mapper, IClient httpclient, ILocalStorageService localStorageService) : base(httpclient, localStorageService)
        {
            this._localStorageService = localStorageService;
            this._mapper = mapper;
            this._httpclient = httpclient;
        }

        public async Task<List<AutomobileVM>> GetMyAutomobileContractsAsync()
        { 
            AddBearerToken();
               var Automobiles = await _client.MycontractsAsync();
            
            return _mapper.Map<List<AutomobileVM>>(Automobiles);
            throw new NotImplementedException();
        }

    }
}