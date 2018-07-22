using System;
using System.ComponentModel.DataAnnotations;

namespace Tethys.Server.Models
{
    public class PushNotification : DomainModelBase
    {
        [Required] public string Key { get; set; }
        public uint Delay { get; set; }
        [Required] public string Body { get; set; }

        public uint NotifyTimes { get; set; }
        public uint NotifiedCounter { get; set; }
        public bool WasNotified { get; set; }
        public DateTime NotifiedOnUtc { get; set; }
        public bool WasFullyHandled => this.NotifiedCounter == this.NotifyTimes;
    }
}
