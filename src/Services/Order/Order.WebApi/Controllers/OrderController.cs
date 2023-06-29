using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Orders.Commands.CancelOrder;
using Order.Application.Features.Orders.Commands.CheckoutOrder;

namespace Order.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class OrderController : BaseController
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CheckoutOrder")]
        //  [ProducesResponseType(typeof(Boolean), 200)]
        public async Task<IActionResult> CheckoutOrder()
        {
            CheckoutOrderCommand command = new CheckoutOrderCommand();
            command.CustomerId = UserId;
            var result = await _mediator.Send(command);

            return  Ok(result);
        }

        //  [HttpPost]
        //  [Authorize]
        ////  [ProducesResponseType(typeof(Boolean), 200)]
        //  public async Task<IActionResult> CancelOrder([FromBody] CancelOrderCommand command)
        //  {
        //      command. = UserId;
        //      var result = await _mediator.Send(command);

        //      return JsonResult(result);
        //  }
    }
}