using System.Collections.Generic;
using System.Threading.Tasks;
using Tethys.Server.Models;

namespace Tethys.Server.Services
{
    public interface INotificationService
    {
        Task NotifyAsync(IEnumerable<PushNotification> notifications);
        void Stop();
    }
}