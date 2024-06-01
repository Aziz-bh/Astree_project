using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using IAuthenticationService = ClientAstree.Contracts.IAuthenticationService;

namespace ClientAstree.Services
{
    public class AuthenticationService : BaseHttpService, IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private JwtSecurityTokenHandler _tokenHandler;

        public AuthenticationService(IClient client, ILocalStorageService localStorage, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(client, localStorage)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._mapper = mapper;
            this._tokenHandler = new JwtSecurityTokenHandler();
        }

        public async Task<bool> Authenticate(string email, string password)
        {
            try
            {
                LoginDto authenticationRequest = new() { Email = email, Password = password };
                var authenticationResponse = await _client.LoginAsync(authenticationRequest);
        // Console.WriteLine($"authenticationResponse: {authenticationResponse.Token}");

                if (authenticationResponse.Token != string.Empty)
                {
                    //Get Claims from token and Build auth user object
                    var tokenContent = _tokenHandler.ReadJwtToken(authenticationResponse.Token);
                    var claims = ParseClaims(tokenContent);
                    var user = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                    Console.WriteLine($"authenticationResponse: {user}");
                     Console.WriteLine($"authenticationResponse: {user}");
                      Console.WriteLine($"authenticationResponse: {user}");
                       Console.WriteLine($"authenticationResponse: {user}");
                               foreach (var claim in user.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

                    if (_httpContextAccessor.HttpContext != null)
                    {
                        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);
                    }

                    _localStorage.SetStorageValue("token", authenticationResponse.Token);

                   
                    return true;
                }
                return false;
            }
            catch 
            {
                
                return false;
            }
        }

    public async Task<bool> Register(RegisterDto registration)
    {
        // Map the view model to DTO
       // RegisterDto registrationRequest = _mapper.Map<RegisterDto>(registration);

        try
        {
            // Make the asynchronous request to the registration API
            var response = await _client.RegisterAsync(registration);

            // Check if the account creation was successful
            if (response!= null) // Assuming 'IsRegistered' is a bool indicating success
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as necessary
            Console.WriteLine($"Registration failed: {ex.Message}");
            return false;
        }
    }

        public async Task Logout()
        {
            _localStorage.ClearStorage(new List<string> { "token" });
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        {


            var claims = tokenContent.Claims.ToList();

            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }
    }
}
