using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace Identity.Application.Services
{
    public class DeleteOrdersService
    {
        private readonly string _url;
        private readonly HubConnection _hubConnection;

        public DeleteOrdersService(IConfiguration configuration)
        {
            _url = configuration["SignalR:Address"];

            var hubConnectionBuilder = new HubConnectionBuilder();
            _hubConnection = hubConnectionBuilder.WithUrl(_url)
                .Build();
        }

        public async Task InvokeOrderHubMethod(string userId)
        {
            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync(Constants.DeleteOrderMethod, userId);
            }
            catch (Exception ex)
            {
                // Handle any connection or invocation errors
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                await _hubConnection.StopAsync();
            }
        }
    }
}
