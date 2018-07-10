using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tethys.Server.Hubs
{
    public class MockHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            base.OnConnectedAsync();
        }
    }
}
