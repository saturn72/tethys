using System.Threading.Tasks;

namespace Tethys.Server.Services.Notifications
{
    public interface INotificationPublisher
    {
        Task ToServerUnderTestClients(string notificationKey, object notificationBody);
        Task ToLogClients(string notificationKey, object notificationBody);
    }
}