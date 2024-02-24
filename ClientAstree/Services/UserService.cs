using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services.Base;

namespace ClientAstree.Services
{
    public class UserService : BaseHttpService,IUserService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IMapper _mapper;
        private readonly IClient _httpclient;
         public UserService(IMapper mapper, IClient httpclient, ILocalStorageService localStorageService) : base(httpclient, localStorageService)
        {
            this._localStorageService = localStorageService;
            this._mapper = mapper;
            this._httpclient = httpclient;
        }
        public async Task<UserVM> GetUserAsync(int id)
        {
            var user = await _client.UsersAsync(id);
            return _mapper.Map<UserVM>(user);
        }

        public async Task<List<UserVM>> GetUsersAsync()
        {
                        // AddBearerToken();
            var users = await _client.UsersAllAsync();
            return _mapper.Map<List<UserVM>>(users);
        }
    }
}
