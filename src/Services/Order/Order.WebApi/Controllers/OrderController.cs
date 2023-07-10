using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Orders.Commands.CancelOrder;
using Order.Application.Features.Orders.Commands.CancelTicket;
using Order.Application.Features.Orders.Commands.CheckoutOrder;
using Order.Application.Features.Orders.Queries.AllOrders;
using Order.Application.Features.Orders.Queries.OrderHistory;
using Order.Application.Features.Orders.Queries.TicketDetailedInfo;
using Order.WebApi.Helpers;

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

        [HttpPost("Checkout")]
        public async Task<IActionResult> CheckoutOrder()
        {
            var command = new CheckoutOrderCommand();
            command.CustomerId = User.Id;
            var result = await _mediator.Send(command);

            return ErrorHandle.HandleResult(result);
        }

        [HttpPost("Cancel")]
        public async Task<IActionResult> CancelOrder(CancelOrderCommand command)
        {
            command.CustomerId = User.Id;
            var result = await _mediator.Send(command);

            return ErrorHandle.HandleResult(result);
        }

        [HttpPost("CancelTicket")]
        public async Task<IActionResult> CancelTicket(CancelTicketCommand command)
        {
            command.CustomerId = User.Id;
            var result = await _mediator.Send(command);

            return ErrorHandle.HandleResult(result);
        }

        [HttpGet("TicketDetails")]
        public async Task<ActionResult> TicketDetails([FromQuery] TicketsDetailedQuery query)
        {
            query.CustomerId = User.Id;
            var result = await _mediator.Send(query);

            return ErrorHandle.HandleResult(result);
        }

        [HttpGet("OrdersHistory")]
        public async Task<ActionResult> OrdersHistory()
        {
            var query = new OrderHistoryQuery();
            query.CustomerId = User.Id;
            var result = await _mediator.Send(query);

            return ErrorHandle.HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AllOrders")]
        public async Task<ActionResult> AllOrders()
        {
            var query = new AllOrdersQuery();
            var result = await _mediator.Send(query);

            return ErrorHandle.HandleResult(result);
        }
    }
}