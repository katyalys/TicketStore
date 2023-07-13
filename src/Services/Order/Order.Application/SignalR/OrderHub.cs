using Microsoft.AspNetCore.SignalR;
using Order.Domain.Entities;
using Order.Domain.Interfaces;
using Order.Domain.Specification.OrderSpecifications;
using Order.Domain.Specification.TicketSpecifications;

namespace Order.Application.SignalR
{
    public class OrderHub : Hub
    {
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly IGenericRepository<Ticket> _ticketRepository;

        public OrderHub(IGenericRepository<OrderTicket> orderRepository,
            IGenericRepository<Ticket> ticketRepository)
        {
            _orderRepository = orderRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task DeleteOrdersByUserId(string userId)
        {
            var spec = new OrderByCustomerSpec(userId);
            var orders = await _orderRepository.ListAsync(spec);
            _orderRepository.DeleteRange(orders);
            await _orderRepository.SaveAsync();

            var orderIds = orders.Select(o => o.Id).ToList();
            var ticketSpec = new TicketsByOrdersListSpec(orderIds);
            var tickets = await _ticketRepository.ListAsync(ticketSpec);
            _ticketRepository.DeleteRange(tickets);
            await _orderRepository.SaveAsync();
        }
    }
}
