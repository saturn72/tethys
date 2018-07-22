using System.Threading.Tasks;

namespace Tethys.Server.Services.Notifications
{
    public interface INotificationPublisher
    {
        Task ToAll(string notificationKey, string notificationBody);
    }
}