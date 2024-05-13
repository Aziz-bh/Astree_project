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
             AddBearerToken();
            var user = await _client.UsersGETAsync(id);

                              var model =
                new UserVM {
                    // Map your user data to the UserVM model
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = user.Roles,
                    CIN = user.Cin.ToString(),
                    BirthDate = user.BirthDate,
                    Nationality = user.Nationality,
                    Gender = user.Gender.ToString(),
                    Civility = user.Civility.ToString()
                    // Continue mapping other fields
                };

            return model;
        }

        public async Task<List<UserVM>> GetUsersAsync()
        {
                         AddBearerToken();
            var users = await _client.UsersAllAsync();
            
            return _mapper.Map<List<UserVM>>(users);
        }

        public async Task<UserVM> ProfileAsync()
        {
              AddBearerToken();
              var user = await _client.ProfileAsync();
                                          var model =
                new UserVM {
                    // Map your user data to the UserVM model
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = user.Roles,
                    CIN = user.Cin.ToString(),
                    BirthDate = user.BirthDate,
                    Nationality = user.Nationality,
                    Gender = user.Gender.ToString(),
                    Civility = user.Civility.ToString()
                    // Continue mapping other fields
                };

            return model;
        }

        public async Task UpdateAsync(UserUpdateDTO body)
        {
             AddBearerToken();
              Console.WriteLine(body.Cin);
               Console.WriteLine(body.Gender);
                Console.WriteLine(body.PhoneNumber);
                 Console.WriteLine(body.Civility);
                  Console.WriteLine(body.BirthDate);
                  Console.WriteLine(body.FirstName);
                  
            await _client.UpdateAsync(body);
        }

        public async Task UsersDELETEAsync(int id)
        {
            AddBearerToken();
            await _client.UsersDELETEAsync(id);
        }
    }
}
