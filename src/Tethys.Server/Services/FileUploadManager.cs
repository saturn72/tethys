using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ServiceStack.Text;
using Tethys.Server.Models;
using Tethys.Server.Services.HttpCalls;
using Tethys.Server.Services.Notifications;

namespace Tethys.Server.Services
{
    public class FileUploadManager : IFileUploadManager
    {
        public FileUploadManager(IHttpCallService httpCallService, INotificationService notificationService)
        {
            _httpCallService = httpCallService;
            _notificationService = notificationService;
        }
        private readonly INotificationService _notificationService;
        private readonly IHttpCallService _httpCallService;

        public async Task LoadSequenceFromStream(IEnumerable<Stream> streams)
        {
            foreach (var s in streams)
            {
                var cur = JsonSerializer.DeserializeFromStream<HttpCallSequence>(s);
                s.Dispose();
                _notificationService.NotifyAsync(cur.PushNotifications);
                await _httpCallService.Register(cur.HttpCalls);
            }
        }
    }
}