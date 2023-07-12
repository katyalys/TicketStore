using Catalog.Application.Dtos;
using Catalog.Application.Interfaces;
using Catalog.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Catalog.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly AuthUserDto _authUserDto;

        public BasketController(IBasketService basketService, AuthUserDto authUserDto)
        {
            _basketService = basketService;
            _authUserDto = authUserDto;
        }

        [Authorize]
        [HttpGet("ViewBasket")]
        public async Task<IActionResult> GetBasketById()
        {
            _authUserDto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basket = await _basketService.GetBasketAsync(_authUserDto.UserId);

            return ErrorHandle.HandleResult(basket);
        }

        [Authorize]
        [HttpPost("AddTicket")]
        public async Task<IActionResult> AddTicketToBasket(int ticketId)
        {
            _authUserDto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basket = await _basketService.AddBasketTicketAsync(ticketId, _authUserDto.UserId);

            return ErrorHandle.HandleResult(basket);
        }

        [Authorize]
        [HttpDelete("DeleteBasket")]
        public async Task<IActionResult> DeleteBasketById()
        {
            _authUserDto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basket = await _basketService.DeleteBasketAsync(_authUserDto.UserId);

            return ErrorHandle.HandleResult(basket);
        }

        [Authorize]
        [HttpDelete("DeleteTicketFromBasket")]
        public async Task<IActionResult> DeleteTicketFromBasketById(int ticketId)
        {
            _authUserDto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updatedBasket = await _basketService.DeleteFromBasketTicketAsync(ticketId, _authUserDto.UserId);

            return ErrorHandle.HandleResult(updatedBasket);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ListAllBaskets")]
        public async Task<IActionResult> ListAllBaskets()
        {
            var baskets = await _basketService.GetAllBasketsAsync();

            return ErrorHandle.HandleResult(baskets);
        }
    }
}
