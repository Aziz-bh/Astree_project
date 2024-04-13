using AutoMapper;
using ClientAstree.Contracts;
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
        
    }
}