using System.Collections.Generic;
using System.Threading.Tasks;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.Services
{
    public interface INotificationService
    {
        Task Notify(IEnumerable<PushNotification> notifications);
        void Stop();
    }
}