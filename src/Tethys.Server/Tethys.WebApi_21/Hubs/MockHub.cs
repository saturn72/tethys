using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tethys.WebApi.Hubs
{
    public class MockHub : Hub
    {
        public async Task SendMessage(string eventName, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
