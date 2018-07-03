using System;
using System.ComponentModel.DataAnnotations;

namespace Tethys.WebApi.Models
{
    public class PushNotification
    {
        [Required] public string Key { get; set; }
        public int Delay { get; set; }
        [Required] public string Body { get; set; }

        public int NotifyTimes { get; set; }
        public int NotificationCounter { get; set; }

        public long Id { get; set; }

        public bool WasNotified { get; set; }
        public DateTime? NotifiedOnUtc { get; set; }
    }
}
