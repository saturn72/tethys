using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tethys.WebApi.Hubs
{
    public class MockHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}
