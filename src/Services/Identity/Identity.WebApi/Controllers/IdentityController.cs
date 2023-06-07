using Identity.Application.Dtos;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityServer;
using Microsoft.AspNetCore.Authorization;
using Duende.IdentityServer;
using AutoMapper;
using Identity.Application.Interfaces;
using Identity.Domain.Entites;
using Identity.Infrastructure.Attributes;

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
            //var claim = HttpContext.User.Claims;
            var result = await _userService.DeleteUser(userId);
            return HandleResult(result);
        }

        [HttpGet("Admin/ListAllUsers")]
        [Authorize(Roles = "Admin")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return HandleResult(users);
        }

        [Authorize(Roles = "Admin")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost("Admin/ChangeUserRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeUserRole(string id, string newRole)
        {
            var user = await _userService.ChangeRole(id, newRole);
            return HandleResult(user);
        }

        [Authorize(Roles = "Admin")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost("Admin/GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetById(id);
            return HandleResult(user);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var result = await _userService.RegisterCustomer(registerUser);
            return HandleResult(result);
        }

        [HttpPost("Token")]
        public async Task<IActionResult> Login(LoginUser loginUser)
        {
            var tokenRes = await _tokenService.GetToken(loginUser);
            return Ok(tokenRes);
        }

        [CustomAuthorization]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(string clientId, string clientSecret, string refreshToken)
        {
            var tokenRes = await _tokenService.GetRefreshedTokenPairAsync(clientId, clientSecret, refreshToken);
            return Ok(tokenRes);
        }

        private IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            var error = result.Errors[0];
            return error.StatusCode switch
            {
                ErrorStatusCode.NotFound => NotFound(error.Message),
                ErrorStatusCode.WrongAction => BadRequest(error.Message),
                ErrorStatusCode.ForbiddenAction => Forbid(error.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}