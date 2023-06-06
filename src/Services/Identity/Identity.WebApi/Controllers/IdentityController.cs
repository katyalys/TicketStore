using Identity.Application.Dtos;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityServer;
using Microsoft.AspNetCore.Authorization;
using Duende.IdentityServer;
using AutoMapper;
using Identity.Application.Interfaces;

namespace Identity.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public IdentityController(IUserService userService, IMapper map, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        //    [Authorize(Policy = "AdminOnly")]
        [Authorize(Roles = "Admin")]
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
                bool result = await _userService.DeleteUser(userId);

                if (!result)
                {
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
            var users = await _userService.GetAllUsers();
            if (users == null)
            {
                return NotFound();
            }
            else
            {
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
            var user = await _userService.ChangeRole(id, newRole);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }
        }

        [Authorize(Roles = "Admin")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost("Admin/GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetById(id);
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
            await _userService.RegisterCustomer(registerUser);
            return Ok();
        }

        [HttpPost("Token")]
        public async Task<IActionResult> Login(LoginUser loginUser)
        {
            var tokenRes = await _tokenService.GetToken(loginUser);
            return Ok(tokenRes);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(string clientId, string clientSecret, string refreshToken)
        {
            var tokenRes = await _tokenService.GetRefreshedTokenPairAsync(clientId, clientSecret, refreshToken);
            return Ok(tokenRes);
        }
    }
}