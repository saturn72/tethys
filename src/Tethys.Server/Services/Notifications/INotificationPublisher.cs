using System.Threading.Tasks;

namespace Tethys.Server.Services.Notifications
{
    public interface INotificationPublisher
    {
        Task ToServerUnderTestClients(string notificationKey, string notificationBody);
        Task ToLogClients(string notificationKey, string notificationBody);
    }
}