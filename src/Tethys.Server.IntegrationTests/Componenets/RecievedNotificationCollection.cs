using System;
using System.Collections.Generic;

namespace Tethys.Server.IntegrationTests.Components
{
        public sealed class RecievedNotification
        {
            public DateTime RecievedOnUtc { get; internal set; }
            public string Key { get; internal set; }
            public string Body { get; internal set; }
        }
}