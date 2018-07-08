using System.Collections.Generic;
using System.Threading.Tasks;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.Services
{
    public interface INotificationService
    {
        Task NotifyAsync(IEnumerable<PushNotification> notifications);
        void Stop();
    }
}