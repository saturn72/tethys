using System.Collections.Generic;

namespace Tethys.Server.Models
{
    public class HttpCallSequence
    {
        public List<HttpCall> HttpCalls { get; set; }
        public List<PushNotification> PushNotifications { get; set; }
    }
}