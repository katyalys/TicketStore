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

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [Authorize]
        [HttpGet("ViewBasket")]
        public async Task<IActionResult> GetBasketById()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basket = await _basketService.GetBasketAsync(userId);

            return ErrorHandle.HandleResult(basket);
        }

        [Authorize]
        [HttpPost("AddTicket")]
        public async Task<IActionResult> AddTicketToBasket(int ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basket = await _basketService.AddBasketTicketAsync(ticketId, userId);

            return ErrorHandle.HandleResult(basket);
        }

        [Authorize]
        [HttpDelete("DeleteBasket")]
        public async Task<IActionResult> DeleteBasketById()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basket = await _basketService.DeleteBasketAsync(userId);

            return ErrorHandle.HandleResult(basket);
        }

        [Authorize]
        [HttpDelete("DeleteTicketFromBasket")]
        public async Task<IActionResult> DeleteTicketFromBasketById(int ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updatedBasket = await _basketService.DeleteFromBasketTicket(ticketId, userId);

            return ErrorHandle.HandleResult(updatedBasket);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ListAllBaskets")]
        public async Task<IActionResult> ListAllBaskets()
        {
            var baskets = await _basketService.GetAllBaskets();

            return ErrorHandle.HandleResult(baskets);
        }
    }
}
