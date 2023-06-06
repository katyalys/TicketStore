using Identity.Application.Dtos;
using Identity.Domain;
using Identity.Infrastructure.Data;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityModel.Client;
using System.Text.Json;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using IdentityServer;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Identity.Application.Services;
using Microsoft.AspNetCore.Authorization;
using IdentityModel;
using Duende.IdentityServer;
using StructureMap;

namespace Identity.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {


        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityDbContext _context;
        private readonly AccountService _accountService;
        private readonly string _url; 

        public WeatherForecastController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IdentityDbContext context, AccountService accountService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _url = configuration["ID4:Authority"];
            _accountService = accountService;
        }

        //    [Authorize(Policy = "AdminOnly")]
        [Authorize(Roles = "Admin")]
       // [Authorize(Policy = "AdminOnly")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost("Admin/DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUserById(string userId)
        {
            var claim = HttpContext.User.Claims;
            if (HttpContext.User.Identity.Name != userId)
            {
                bool result = await _accountService.DeleteUser(userId);

                if (!result)
                {
                    //throw new Exception("User not found");
                    return NotFound();
                }
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("Admin/ListAllUsers")]
        [Authorize(Roles = "Admin")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListAllUsers()
        {
            var users = await _accountService.GetAllUsers();
            if (users == null)
            {
                return NotFound();
            }
            else {
                return Ok(users); 
            }
        }

        [Authorize(Roles = "Admin")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost("Admin/ChangeUserRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeUserRole(string id, string newRole)
        {
            var user = await _accountService.ChangeRole(id, newRole);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(user); 
            }
        }



        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var r = await _accountService.RegisterCustomer(registerUser);
            return Content("ass");
        }

        [HttpPost("Token")]
        public async Task<IActionResult> Login(LoginUser loginUser)
        {
            //var client = new HttpClient();
            //// var discoveryDocument = await client.GetDiscoveryDocumentAsync("http://localhost:5012");

            //var tokenClient = new TokenClient(client, new TokenClientOptions
            //{
            //    Address = "http://localhost:5012/connect/token",
            //    ClientId = "identityUserClient",
            //    ClientSecret = "IdentityApiSecret"
            //});

            ////var tokenResponse = await tokenClient.RequestPasswordTokenAsync(JsonSerializer.Serialize(new PasswordTokenRequest
            ////{
            ////    UserName = loginUser.UserName,
            ////    Password = loginUser.Password,
            ////    Scope = "Identity.User"
            ////}));

            //var json = JsonSerializer.Serialize(tokenResponse);

            //return Content(json, "application/json");

            //var accessToken = await HttpContext.GetTokenAsync("access_token");

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //var content = await client.GetStringAsync("http://localhost:5012/identity");

            //return Content(content, "application/json");

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://localhost:5012",
                Policy = new DiscoveryPolicy { RequireHttps = false,  }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            //string[] values = { "Identity.Offline_access", "Identity" };
            //List<string> list = values.ToList();

            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = "identityUserClient",
                ClientSecret = "IdentityApiSecret",
                UserName = loginUser.UserName,
                Password = loginUser.Password,
                Address = disco.TokenEndpoint,
                Scope = "Identity Identity.Offline_access IdentityServerApi",
                //Resource = list,

            };

            //This is how I get token with ResourceOwnerPassword
            var token = await client.RequestPasswordTokenAsync(passwordTokenRequest);

            var json = JsonSerializer.Serialize(token);
            return Content(json, "application/json");
        }

    }
}