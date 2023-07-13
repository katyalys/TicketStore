using Identity.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Identity.Application.SignalR
{
    public class DeletedUserHub : Hub
    {
        public async Task DeletedUsers(string userId)
        {
            await Clients.All.SendAsync("DeleteOrdersByUserId", userId);
        }
    }
}
