using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Dtos;
using Order.Application.Features.Orders.Commands.CancelOrder;
using Order.Application.Features.Orders.Commands.CancelTicket;
using Order.Application.Features.Orders.Commands.CheckoutOrder;
using Order.Application.Features.Orders.Queries.AllOrders;
using Order.Application.Features.Orders.Queries.OrderHistory;
using Order.Application.Features.Orders.Queries.TicketDetailedInfo;

namespace Order.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : BaseController
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("CheckoutOrder")]
        //  [ProducesResponseType(typeof(Boolean), 200)]
        public async Task<IActionResult> CheckoutOrder()
        {
            CheckoutOrderCommand command = new CheckoutOrderCommand();
            command.CustomerId = UserId;
            var result = await _mediator.Send(command);

            return  Ok(result);
        }

        [Authorize]
        [HttpPost("CancelOrder")]
        //  [ProducesResponseType(typeof(Boolean), 200)]
        public async Task<IActionResult> CancelOrder(CancelOrderCommand command)
        {
            command.CustomerId = UserId;
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("CancelTicket")]
        //  [ProducesResponseType(typeof(Boolean), 200)]
        public async Task<IActionResult> CancelTicket(CancelTicketCommand command)
        {
            command.CustomerId = UserId;
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("TicketDetails")]
        //  [ProducesResponseType(typeof(Boolean), 200)]
        public async Task<ActionResult<List<TicketDetailInfo>?>> TicketDetails([FromQuery]TicketsDetailedQuery query)
        {
            query.CustomerId = UserId;
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("OrdersHistory")]
        //  [ProducesResponseType(typeof(Boolean), 200)]
        public async Task<ActionResult<List<TicketDetailInfo>?>> OrdersHistory()
        {
            OrderHistoryQuery query = new OrderHistoryQuery();
            query.CustomerId = UserId;
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AllOrders")]
        //  [ProducesResponseType(typeof(Boolean), 200)]
        public async Task<ActionResult<List<TicketDetailInfo>?>> AllOrders()
        {
            AllOrdersQuery query = new AllOrdersQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
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