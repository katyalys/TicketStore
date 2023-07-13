using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace Identity.Application.Services
{
    public class DeleteOrdersService
    {
        private readonly string _url;

        public DeleteOrdersService(IConfiguration configuration)
        {
            _url = configuration["SignalR:Address"];
        }

        public async Task InvokeOrderHubMethod(string userId)
        {
            var hubConnectionBuilder = new HubConnectionBuilder();
            var hubConnection = hubConnectionBuilder.WithUrl(_url)
                .Build();

            try
            {
                await hubConnection.StartAsync();
                await hubConnection.InvokeAsync("DeleteOrdersByUserId", userId);
            }
            catch (Exception ex)
            {
                // Handle any connection or invocation errors
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                await hubConnection.StopAsync();
            }
        }
    }
}
