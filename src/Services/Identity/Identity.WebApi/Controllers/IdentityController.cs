using Identity.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using IdentityServer;
using Microsoft.AspNetCore.Authorization;
using Duende.IdentityServer;
using AutoMapper;
using Identity.Application.Interfaces;
using Identity.Infrastructure.Attributes;
using Identity.WebApi.Helpers;
using Identity.Application.Services;
using Identity.Domain.ErrorModels;

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

        [Authorize(Roles = "Admin")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost("DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUserById(string userId)
        {
            var result = await _userService.DeleteUser(userId);

            return ErrorHandle.HandleResult(result);
        }

        [HttpGet("ListAllUsers")]
        [Authorize(Roles = "Admin")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListAllUsers()
        {
            var users = await _userService.GetAllUsers();

            return ErrorHandle.HandleResult(users);
        }

        [Authorize(Roles = "Admin")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost("ChangeUserRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeUserRole(string id, string newRole)
        {
            var user = await _userService.ChangeRole(id, newRole);

            return ErrorHandle.HandleResult(user);
        }

        [Authorize(Roles = "Admin")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost("GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetById(id);

            return ErrorHandle.HandleResult(user);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var result = await _userService.RegisterCustomer(registerUser);

            return ErrorHandle.HandleResult(result);
        }

        [HttpPost("Token")]
        public async Task<IActionResult> Login(LoginUser loginUser)
        {
            var tokenRes = await _tokenService.GetToken(loginUser);

            return ErrorHandle.HandleResult(tokenRes);
        }

        [CustomAuthorization]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(string clientId, string clientSecret, string refreshToken)
        {
            var tokenRes = await _tokenService.GetRefreshedTokenPairAsync(clientId, clientSecret, refreshToken);

            return Ok(tokenRes);
        }
    }
}