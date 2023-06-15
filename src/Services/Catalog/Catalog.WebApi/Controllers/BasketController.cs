using AutoMapper;
using Catalog.Application.Dtos.BasketDtos;
using Catalog.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IMapper _mapper;
        public BasketController(IBasketService basketService, IMapper mapper)
        {
            _mapper = mapper;
            _basketService = basketService;
        }

        [HttpGet]
        public async Task<ActionResult<BasketDto>> GetBasketById()
        {
            var userId = User.FindFirst("sub")?.Value;
            var basket = await _basketService.GetBasketAsync(userId);

            return Ok(basket);
        }


    }
}
