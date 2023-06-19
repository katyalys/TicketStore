using AutoMapper;
using Catalog.Application.Dtos.BasketDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Catalog.WebApi.Controllers
{
    // [Authorize]
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IMapper _mapper;
        public BasketController(IMapper mapper, IBasketService basketService)
        {
            _mapper = mapper;
            _basketService = basketService;
        }

        [Authorize]
        [HttpGet("ViewBasket")]
        public async Task<ActionResult<BasketDto>> GetBasketById()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basket = await _basketService.GetBasketAsync(userId);

            return Ok(basket);
        }

        [Authorize]
        [HttpGet("AddTicket")]
        public async Task<ActionResult<BasketDto>> AddTicketToBasket(int ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var basket = await _basketService.AddBasketTicketAsync(ticketId, userId);

            return Ok(basket);
        }

        [Authorize]
        [HttpDelete("DeleteBasket")]
        public async Task<ActionResult<BasketDto>> DeleteBasketById()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _basketService.DeleteBasketAsync(userId);

            return Ok();
        }

        [Authorize]
        [HttpDelete("DeleteTicketFromBasket")]
        public async Task<ActionResult<BasketDto>> DeleteTicketFromBasketById(int ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updatedBasket = await _basketService.DeleteFromBasketTicket(ticketId, userId);

            return Ok(updatedBasket);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ListAllBaskets")]
        public async Task<ActionResult<Dictionary<string, Basket>>> ListAllBaskets()
        {
            var baskets = await _basketService.GetAllBaskets();

            return Ok(baskets);
        }
    }
}
