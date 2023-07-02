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
        public async Task<IActionResult> CheckoutOrder()
        {
            CheckoutOrderCommand command = new CheckoutOrderCommand();
            command.CustomerId = UserId;
            var result = await _mediator.Send(command);
            return ErrorHandle.HandleResult(result);
        }

        [Authorize]
        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(CancelOrderCommand command)
        {
            command.CustomerId = UserId;
            var result = await _mediator.Send(command);
            return ErrorHandle.HandleResult(result);
        }

        [Authorize]
        [HttpPost("CancelTicket")]
        public async Task<IActionResult> CancelTicket(CancelTicketCommand command)
        {
            command.CustomerId = UserId;
            var result = await _mediator.Send(command);
            return ErrorHandle.HandleResult(result);
        }

        [Authorize]
        [HttpGet("TicketDetails")]
        public async Task<ActionResult> TicketDetails([FromQuery] TicketsDetailedQuery query)
        {
            query.CustomerId = UserId;
            var result = await _mediator.Send(query);
            return ErrorHandle.HandleResult(result);
        }

        [Authorize]
        [HttpGet("OrdersHistory")]
        public async Task<ActionResult> OrdersHistory()
        {
            OrderHistoryQuery query = new OrderHistoryQuery();
            query.CustomerId = UserId;
            var result = await _mediator.Send(query);
            return ErrorHandle.HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AllOrders")]
        public async Task<ActionResult> AllOrders()
        {
            AllOrdersQuery query = new AllOrdersQuery();
            var result = await _mediator.Send(query);
            return ErrorHandle.HandleResult(result);
        }
    }
}