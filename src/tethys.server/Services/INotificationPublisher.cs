using System.Threading.Tasks;

namespace Tethys.Server.Services
{
    public interface INotificationPublisher
    {
        Task ToAll(string notificationKey, string notificationBody);
    }
}